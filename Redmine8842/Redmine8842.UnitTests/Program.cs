using System;
using System.IO;

namespace Redmine8842.UnitTests
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            string sourceRoot = ResolveSourceRoot(args);

            Console.WriteLine("Redmine 8842 Unit Tests");
            Console.WriteLine("SourceRoot: " + sourceRoot);
            Console.WriteLine();

            TestRunner runner = new TestRunner();
            SourceRegressionTests tests = new SourceRegressionTests(sourceRoot);

            runner.Add("TC-01 UI cancel button rejects unsaved slip", tests.UiCancelButtonRejectsUnsavedSlip);
            runner.Add("TC-02/TC-03 UI cancel button keeps logical delete flags off", tests.UiCancelButtonSetsCancelModeAndPreservesLogicalDeleteOff);
            runner.Add("TC-14 UI confirm flow redirects without executing delete", tests.UiConfirmFlowRedirectsWithoutExecutingDelete);
            runner.Add("TC-15 UI direct flow executes delete facade and shows completion", tests.UiDirectFlowExecutesDeleteFacadeAndShowsCompletion);
            runner.Add("TC-11/TC-12 UI search result keeps canceled slips visible and hides correction button", tests.UiSearchResultKeepsCanceledSlipsVisibleAndHidesCorrectionButton);
            runner.Add("TC-02/TC-03 Business cancel flow creates red slip and does not call logical delete", tests.BusinessCancelFlowCreatesRedSlipAndDoesNotDeleteOriginal);
            runner.Add("TC-02 Business error cancel flow does not call error logical delete", tests.BusinessErrorCancelFlowDoesNotDeleteErrorTran);
            runner.Add("TC-04/TC-05 SARSlipHeader red slip SQL follows Redmine 8842", tests.HeaderRedSlipSqlMatchesSpec);
            runner.Add("TC-06/TC-07 SARSlipDetail red slip SQL follows Redmine 8842", tests.DetailRedSlipSqlMatchesSpec);
            runner.Add("TC-09 SARSlipHeader SlipRevision update SQL exists", tests.SlipRevisionUpdateSqlMatchesSpec);
            runner.Add("TC-10 SARSlipHeaderError FixDate update SQL exists", tests.FixDateUpdateSqlMatchesSpec);
            runner.Add("TC-16/TC-17 AccountBook flow is not directly changed by Redmine 8842", tests.AccountBookFlowIsNotDirectlyChanged);
            runner.Add("Test data TD-02 red slip values offset original values", tests.TestDataOffsetsAreCorrect);
            runner.Add("Modified source methods have balanced braces", tests.ModifiedMethodsHaveBalancedBraces);

            return runner.RunAll();
        }

        private static string ResolveSourceRoot(string[] args)
        {
            if (args != null && args.Length > 0 && Directory.Exists(args[0]))
            {
                return Path.GetFullPath(args[0]);
            }

            string env = Environment.GetEnvironmentVariable("REDMINE8842_INPUTSALESSLIP_ROOT");
            if (!string.IsNullOrEmpty(env) && Directory.Exists(env))
            {
                return Path.GetFullPath(env);
            }

            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, "InputSalesSlip.Business")))
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            string defaultRoot = @"C:\CCRSourceCode\Branch2.2\CoreCenterRetail\Branch2.2\Retail\Unisys\Sales\InputSalesSlip";
            if (Directory.Exists(defaultRoot))
            {
                return defaultRoot;
            }

            throw new DirectoryNotFoundException("InputSalesSlip source root was not found. Pass it as the first argument or set REDMINE8842_INPUTSALESSLIP_ROOT.");
        }
    }
}
