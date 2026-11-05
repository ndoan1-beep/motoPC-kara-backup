using System;
using System.IO;

namespace Redmine8842.UnitTests
{
    internal sealed class SourceRegressionTests
    {
        private readonly string sourceRoot;
        private readonly string businessPath;
        private readonly string daoPath;
        private readonly string inputUiPath;
        private readonly string searchResultUiPath;
        private readonly string slipHeaderListPath;

        public SourceRegressionTests(string sourceRoot)
        {
            this.sourceRoot = sourceRoot;
            businessPath = Path.Combine(sourceRoot, "InputSalesSlip.Business", "SalesChangeDisposalSlipEntryNewBusiness.cs");
            daoPath = Path.Combine(sourceRoot, "InputSalesSlip.Business", "SalesChangeDisposalSlipEntryNewDataAccessComponent.cs");
            inputUiPath = Path.Combine(sourceRoot, "InputSalesSlip.Web", "Sales", "InputSalesSlip", "SalesChangeDisposalSlipInput", "SalesChangeDisposalSlipInputUpdate_partial.cs");
            searchResultUiPath = Path.Combine(sourceRoot, "InputSalesSlip.Web", "Sales", "InputSalesSlip", "SalesChangeDisposalSlip", "SalesChangeDisposalSlipSearchResult_partial.cs");
            slipHeaderListPath = Path.Combine(sourceRoot, "InputSalesSlip.Business", "SlipHeaderListDataSet.cs");
        }

        public void UiCancelButtonRejectsUnsavedSlip()
        {
            string method = ExtractMethod(ReadRequired(inputUiPath), "protected void SlipDeleateButton_Click(");
            string unsavedBlock = ExtractBlock(method, "if (dsEntity.SARSlipHeaderErrorDataset.SARSlipHeaderError.Rows.Count <= 0)");

            AssertContains(unsavedBlock, "ErrorKey.ERRCSARB0273", "Unsaved slip cancel must show ERRCSARB0273.");
            AssertContains(unsavedBlock, "return;", "Unsaved slip cancel must stop before facade execution.");
            AssertNotContains(unsavedBlock, "DelSARSlipNew", "Unsaved slip cancel must not call delete facade.");
        }

        public void UiCancelButtonSetsCancelModeAndPreservesLogicalDeleteOff()
        {
            string method = ExtractMethod(ReadRequired(inputUiPath), "protected void SlipDeleateButton_Click(");

            AssertContains(method, "headerRow.LogicalDelFlg = \"0\";", "UI cancel must keep header error LogicalDelFlg off.");
            AssertContains(method, "detailRow.LogicalDelFlg = \"0\";", "UI cancel must keep detail error LogicalDelFlg off.");
            AssertContains(method, "workEntity.CancelMode = Constants.OnFlag;", "UI cancel must set CancelMode on before facade execution.");
        }

        public void UiConfirmFlowRedirectsWithoutExecutingDelete()
        {
            string method = ExtractMethod(ReadRequired(inputUiPath), "protected void SlipDeleateButton_Click(");
            string confirmBlock = ExtractBlock(method, "if (isDisPlayOff)");

            AssertContains(confirmBlock, "workEntity.ButtonId = \"5\";", "Confirm flow must set cancel button id.");
            AssertContains(confirmBlock, "this.Redirect(\"SCRCSAR050208\", null);", "Confirm flow must redirect to confirmation screen.");
            AssertNotContains(confirmBlock, "DelSARSlipNew", "Confirm flow must not execute delete facade before confirmation.");
        }

        public void UiDirectFlowExecutesDeleteFacadeAndShowsCompletion()
        {
            string method = ExtractMethod(ReadRequired(inputUiPath), "protected void SlipDeleateButton_Click(");

            AssertContains(method, "this.GetFacadeInstance().DelSARSlipNew(workEntity, dsEntity, initialDisplayEntity, coverSheetMasterEntity);", "Direct flow must call DelSARSlipNew.");
            AssertContains(method, "this.GetFacadeInstance().FillSARSlipNew(workEntity, dsInputStatusEntity);", "Direct flow must refresh slip list after cancel.");
            AssertContains(method, "workEntity.CancelMode = Constants.OffFlag;", "UI cancel must reset CancelMode after execution.");
            AssertContains(method, "MessageKey.MSGCSARI0028", "UI cancel must show completion message after direct execution.");
        }

        public void UiSearchResultKeepsCanceledSlipsVisibleAndHidesCorrectionButton()
        {
            string query = ExtractMethod(ReadRequired(slipHeaderListPath), "private string CreateFillQuery(");
            string searchPage = ReadRequired(searchResultUiPath);

            AssertContains(query, "sqlBuilder.Append(\" AND HD.LogicalDelFlg <> '1' \");", "Header search must include non-deleted original/red slips.");
            AssertContains(query, "sqlBuilder.Append(\" AND HD.SlipRevisionFlg <> '1' \");", "Search condition must control whether revised slips are hidden.");
            AssertContains(query, "sqlBuilder.Append(\" ,CASE WHEN HD.SlipRevisionFlg <> '1' \");", "Header rows must expose correction availability from SlipRevisionFlg.");
            AssertContains(query, "sqlBuilder.Append(\" ,CASE WHEN (FixDate IS NULL)\");", "Error rows must expose correction availability from FixDate.");
            AssertContains(searchPage, "Rows[i][\"SlipCorrectionAvailabilityFlg\"].ToString().Equals(\"0\")", "Search result must check correction availability flag.");
            AssertContains(searchPage, "SARSlipHeader_SlipCorrectionButton_Control\")).Visible = false;", "Search result must hide correction button for unavailable slips.");
        }

        public void BusinessCancelFlowCreatesRedSlipAndDoesNotDeleteOriginal()
        {
            string method = ExtractMethod(ReadRequired(businessPath), "public virtual void UpInsSlipNew(");
            string cancelBlock = ExtractBlock(method, "if (workEntity.CancelMode == Constants.OnFlag)");

            AssertContains(cancelBlock, "workEntity.SlipUpdateMode = slipUpdateMode_Red;", "Cancel flow must set red slip update mode.");
            AssertContains(cancelBlock, "this.UpInsSlipExecuteForDenyNew(workEntity, dsEntity);", "Cancel flow must create a red slip.");
            AssertContains(cancelBlock, "dataAccessComponent.UpdateSlipRevisionFlgSARSlipHeaderNew(workEntity, dsEntity);", "Cancel flow must update SlipRevisionFlg.");
            AssertContains(cancelBlock, "dataAccessComponent.UpdateFixDateSARSlipHeaderErrorNew(workEntity, dsEntity);", "Cancel flow must update header error FixDate.");
            AssertNotContains(cancelBlock, "UpdateDelFlgSARSlipHeaderNew", "Cancel flow must not logically delete SARSlipHeader.");
            AssertNotContains(cancelBlock, "UpdateDelFlgSARSlipDetailNew", "Cancel flow must not logically delete SARSlipDetail.");
        }

        public void BusinessErrorCancelFlowDoesNotDeleteErrorTran()
        {
            string method = ExtractMethod(ReadRequired(businessPath), "public virtual void UpInsSlipErrorExecuteNew(");
            string cancelBlock = ExtractBlock(method, "if (workEntity.CancelMode == Constants.OnFlag)");

            AssertNotContains(cancelBlock, "UpdateDelFlgSARSlipHeaderErrorNew", "Cancel flow must not logically delete SARSlipHeaderError.");
            AssertNotContains(cancelBlock, "UpdateDelFlgSARSlipDetailErrorNew", "Cancel flow must not logically delete SARSlipDetailError.");
            AssertContains(method, "UpInsSARSlipHeaderErrorNew(workEntity, dsEntity);", "Normal flow must still upsert header error.");
            AssertContains(method, "UpInsSARSlipDetailErrorNew(workEntity, dsEntity);", "Normal flow must still upsert detail error.");
        }

        public void HeaderRedSlipSqlMatchesSpec()
        {
            string method = ExtractMethod(ReadRequired(daoPath), "private string CreateInsertSARSlipHeaderForDenyQueryTextNew()");
            string selectPart = After(method, "insertCommandText.AppendLine(\"SELECT\");");
            string columnPart = Before(method, "insertCommandText.AppendLine(\"SELECT\");");

            AssertContains(columnPart, "insertCommandText.AppendLine(\"   ,TotalQty\");", "INSERT column list must keep TotalQty as a column name.");
            AssertContains(columnPart, "insertCommandText.AppendLine(\"   ,SlipTotalDisposalAmt\");", "INSERT column list must keep SlipTotalDisposalAmt as a column name.");
            AssertContains(columnPart, "insertCommandText.AppendLine(\"   ,SlipRevisionFlg\");", "INSERT column list must keep SlipRevisionFlg as a column name.");
            AssertContains(columnPart, "insertCommandText.AppendLine(\"   ,LogicalDelFlg\");", "INSERT column list must keep LogicalDelFlg as a column name.");

            AssertContains(selectPart, "insertCommandText.AppendLine(\"   ,-1 * TotalQty\");", "Red header TotalQty must be negative.");
            AssertContains(selectPart, "insertCommandText.AppendLine(\"   ,-1 * SlipTotalDisposalAmt\");", "Red header SlipTotalDisposalAmt must be negative.");
            AssertContains(selectPart, "insertCommandText.AppendLine(\"   ,'1'\");", "Red header must set SlipRevisionFlg/RedBlackDiv to '1'.");
            AssertContains(selectPart, "insertCommandText.AppendLine(\"   ,'0'\");", "Red header LogicalDelFlg must be '0'.");
        }

        public void DetailRedSlipSqlMatchesSpec()
        {
            string method = ExtractMethod(ReadRequired(daoPath), "private string CreateInsertSARSlipDetailForDenyFillQueryNew()");
            string selectPart = After(method, "insertCommandText.AppendLine(\"SELECT\");");
            string columnPart = Before(method, "insertCommandText.AppendLine(\"SELECT\");");

            AssertContains(columnPart, "insertCommandText.AppendLine(\"    DisposalChangeQty,\");", "INSERT column list must keep DisposalChangeQty as a column name.");
            AssertContains(columnPart, "insertCommandText.AppendLine(\"    DisposalPriceAmt,\");", "INSERT column list must keep DisposalPriceAmt as a column name.");
            AssertContains(columnPart, "insertCommandText.AppendLine(\"    LogicalDelFlg,\");", "INSERT column list must keep LogicalDelFlg as a column name.");

            AssertContains(selectPart, "insertCommandText.AppendLine(\"    -1 * DisposalChangeQty,\");", "Red detail DisposalChangeQty must be negative.");
            AssertContains(selectPart, "insertCommandText.AppendLine(\"    -1 * DisposalPriceAmt,\");", "Red detail DisposalPriceAmt must be negative.");
            AssertContains(selectPart, "insertCommandText.AppendLine(\"    '0',\");", "Red detail LogicalDelFlg must be '0'.");
        }

        public void SlipRevisionUpdateSqlMatchesSpec()
        {
            string method = ExtractMethod(ReadRequired(daoPath), "public virtual void UpdateSlipRevisionFlgSARSlipHeaderNew(");

            AssertContains(method, "UPDATE SARSlipHeader", "SlipRevision update must target SARSlipHeader.");
            AssertContains(method, "SET SlipRevisionFlg = '1'", "SlipRevisionFlg must be set to '1'.");
            AssertContains(method, "AND  LogicalDelFlg = '0'", "SlipRevision update must keep logical delete off.");
            AssertContains(method, "CreateUpdateSlipRevisionFlgSARSlipHeaderParameterListNew", "SlipRevision update must use its parameter list.");
            AssertNotContains(method, "SlipBranchNo", "SlipRevision update must not filter by SlipBranchNo, so original and red slips are both updated.");
        }

        public void FixDateUpdateSqlMatchesSpec()
        {
            string method = ExtractMethod(ReadRequired(daoPath), "public virtual void UpdateFixDateSARSlipHeaderErrorNew(");

            AssertContains(method, "UPDATE SARSlipHeaderError", "FixDate update must target SARSlipHeaderError.");
            AssertContains(method, "SET FixDate = '9999/12/31'", "FixDate must be set to 9999/12/31.");
            AssertContains(method, "CreateUpdateFixDateSARSlipHeaderErrorParameterListNew", "FixDate update must use its parameter list.");
            AssertNotContains(method, "LogicalDelFlg = '1'", "FixDate update must not logically delete header error.");
            AssertNotContains(method, "LogicalDelDate", "FixDate update must not update LogicalDelDate.");
        }

        public void AccountBookFlowIsNotDirectlyChanged()
        {
            string method = ExtractMethod(ReadRequired(businessPath), "public virtual void UpInsAccountBookNew(");

            AssertNotContains(method, "UpdateSlipRevisionFlgSARSlipHeaderNew", "AccountBook flow must not call Redmine 8842 header update directly.");
            AssertNotContains(method, "UpdateFixDateSARSlipHeaderErrorNew", "AccountBook flow must not call Redmine 8842 FixDate update directly.");
            AssertContains(method, "AccountBook", "The existing account book flow must still exist.");
        }

        public void TestDataOffsetsAreCorrect()
        {
            AssertEqual(0m, Redmine8842TestData.OriginalTotalQty + Redmine8842TestData.RedTotalQty, "Header TotalQty must offset to zero.");
            AssertEqual(0m, Redmine8842TestData.OriginalSlipTotalDisposalAmt + Redmine8842TestData.RedSlipTotalDisposalAmt, "Header SlipTotalDisposalAmt must offset to zero.");
            AssertEqual(0m, Redmine8842TestData.Detail1OriginalQty + Redmine8842TestData.Detail1RedQty, "Detail line 1 qty must offset to zero.");
            AssertEqual(0m, Redmine8842TestData.Detail1OriginalAmt + Redmine8842TestData.Detail1RedAmt, "Detail line 1 amount must offset to zero.");
            AssertEqual(0m, Redmine8842TestData.Detail2OriginalQty + Redmine8842TestData.Detail2RedQty, "Detail line 2 qty must offset to zero.");
            AssertEqual(0m, Redmine8842TestData.Detail2OriginalAmt + Redmine8842TestData.Detail2RedAmt, "Detail line 2 amount must offset to zero.");
            AssertEqual("0", Redmine8842TestData.LogicalDelFlgOff, "LogicalDelFlg expected value must be '0'.");
            AssertEqual("003", Redmine8842TestData.AccountBookDataId, "CMCAccountBook DataID must remain '003'.");
        }

        public void ModifiedMethodsHaveBalancedBraces()
        {
            string business = ReadRequired(businessPath);
            string dao = ReadRequired(daoPath);

            ExtractMethod(business, "public virtual void UpInsSlipNew(");
            ExtractMethod(business, "public virtual void UpInsSlipErrorExecuteNew(");
            ExtractMethod(business, "public virtual void UpInsAccountBookNew(");
            ExtractMethod(dao, "public virtual void UpdateSlipRevisionFlgSARSlipHeaderNew(");
            ExtractMethod(dao, "public virtual void UpdateFixDateSARSlipHeaderErrorNew(");
            ExtractMethod(dao, "private string CreateInsertSARSlipHeaderForDenyQueryTextNew()");
            ExtractMethod(dao, "private string CreateInsertSARSlipDetailForDenyFillQueryNew()");
            ExtractMethod(ReadRequired(inputUiPath), "protected void SlipDeleateButton_Click(");
            ExtractMethod(ReadRequired(slipHeaderListPath), "private string CreateFillQuery(");
        }

        private static string ReadRequired(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Required source file was not found.", path);
            }

            return File.ReadAllText(path);
        }

        private static string ExtractMethod(string text, string signature)
        {
            int signatureIndex = text.IndexOf(signature, StringComparison.Ordinal);
            if (signatureIndex < 0)
            {
                throw new InvalidOperationException("Method signature was not found: " + signature);
            }

            int openBrace = text.IndexOf('{', signatureIndex);
            if (openBrace < 0)
            {
                throw new InvalidOperationException("Method open brace was not found: " + signature);
            }

            int closeBrace = FindMatchingBrace(text, openBrace);
            if (closeBrace >= 0)
            {
                return text.Substring(signatureIndex, closeBrace - signatureIndex + 1);
            }

            int fallbackEnd = FindNextMemberStart(text, openBrace + 1);
            if (fallbackEnd > signatureIndex)
            {
                return text.Substring(signatureIndex, fallbackEnd - signatureIndex);
            }

            throw new InvalidOperationException("Method close brace was not found: " + signature);
        }

        private static string ExtractBlock(string text, string marker)
        {
            int markerIndex = text.IndexOf(marker, StringComparison.Ordinal);
            if (markerIndex < 0)
            {
                throw new InvalidOperationException("Block marker was not found: " + marker);
            }

            int openBrace = text.IndexOf('{', markerIndex);
            if (openBrace < 0)
            {
                throw new InvalidOperationException("Block open brace was not found: " + marker);
            }

            int closeBrace = FindMatchingBrace(text, openBrace);
            if (closeBrace >= 0)
            {
                return text.Substring(openBrace, closeBrace - openBrace + 1);
            }

            throw new InvalidOperationException("Block close brace was not found: " + marker);
        }

        private static int FindMatchingBrace(string text, int openBrace)
        {
            int depth = 0;
            bool inLineComment = false;
            bool inBlockComment = false;
            bool inString = false;
            bool inVerbatimString = false;
            bool inChar = false;

            for (int i = openBrace; i < text.Length; i++)
            {
                char c = text[i];
                char next = i + 1 < text.Length ? text[i + 1] : '\0';

                if (inLineComment)
                {
                    if (c == '\r' || c == '\n')
                    {
                        inLineComment = false;
                    }

                    continue;
                }

                if (inBlockComment)
                {
                    if (c == '*' && next == '/')
                    {
                        inBlockComment = false;
                        i++;
                    }

                    continue;
                }

                if (inString)
                {
                    if (inVerbatimString)
                    {
                        if (c == '"' && next == '"')
                        {
                            i++;
                        }
                        else if (c == '"')
                        {
                            inString = false;
                            inVerbatimString = false;
                        }
                    }
                    else
                    {
                        if (c == '\\')
                        {
                            i++;
                        }
                        else if (c == '"')
                        {
                            inString = false;
                        }
                    }

                    continue;
                }

                if (inChar)
                {
                    if (c == '\\')
                    {
                        i++;
                    }
                    else if (c == '\'')
                    {
                        inChar = false;
                    }

                    continue;
                }

                if (c == '/' && next == '/')
                {
                    inLineComment = true;
                    i++;
                    continue;
                }

                if (c == '/' && next == '*')
                {
                    inBlockComment = true;
                    i++;
                    continue;
                }

                if (c == '@' && next == '"')
                {
                    inString = true;
                    inVerbatimString = true;
                    i++;
                    continue;
                }

                if (c == '"')
                {
                    inString = true;
                    continue;
                }

                if (c == '\'')
                {
                    inChar = true;
                    continue;
                }

                if (c == '{')
                {
                    depth++;
                }
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
        private static int FindNextMemberStart(string text, int startIndex)
        {
            int lineStart = startIndex;
            while (lineStart >= 0 && lineStart < text.Length)
            {
                int nextLineStart = IndexAfterNextLineBreak(text, lineStart);
                string line = text.Substring(lineStart, (nextLineStart < 0 ? text.Length : nextLineStart) - lineStart).TrimEnd('\r', '\n');

                if (IsTopLevelMemberLine(line))
                {
                    return lineStart;
                }

                lineStart = nextLineStart;
            }

            return -1;
        }

        private static int IndexAfterNextLineBreak(string text, int startIndex)
        {
            int lineFeed = text.IndexOf('\n', startIndex);
            if (lineFeed < 0)
            {
                return -1;
            }

            return lineFeed + 1;
        }

        private static bool IsTopLevelMemberLine(string line)
        {
            string[] prefixes = new string[]
            {
                "        public ",
                "        private ",
                "        protected ",
                "        internal "
            };

            foreach (string prefix in prefixes)
            {
                if (line.StartsWith(prefix, StringComparison.Ordinal) &&
                    line.IndexOf('(') >= 0 &&
                    !line.TrimStart().StartsWith("public partial class", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
        private static string Before(string text, string marker)
        {
            int index = text.IndexOf(marker, StringComparison.Ordinal);
            if (index < 0)
            {
                throw new InvalidOperationException("Marker was not found: " + marker);
            }

            return text.Substring(0, index);
        }

        private static string After(string text, string marker)
        {
            int index = text.IndexOf(marker, StringComparison.Ordinal);
            if (index < 0)
            {
                throw new InvalidOperationException("Marker was not found: " + marker);
            }

            return text.Substring(index + marker.Length);
        }

        private static void AssertContains(string text, string expected, string message)
        {
            if (text.IndexOf(expected, StringComparison.Ordinal) < 0)
            {
                throw new InvalidOperationException(message + " Missing: " + expected);
            }
        }

        private static void AssertNotContains(string text, string unexpected, string message)
        {
            if (text.IndexOf(unexpected, StringComparison.Ordinal) >= 0)
            {
                throw new InvalidOperationException(message + " Unexpected: " + unexpected);
            }
        }

        private static void AssertEqual<T>(T expected, T actual, string message)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException(message + " Expected: " + expected + ", Actual: " + actual);
            }
        }
    }
}
