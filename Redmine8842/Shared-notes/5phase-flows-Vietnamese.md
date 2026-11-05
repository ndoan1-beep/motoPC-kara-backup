#0 Phase 0
 - Quy tắc workflow
 - Prompt contract
 - Coding conventions
 - SQL conventions
 - Naming rules
 - Folder rules
 - Encoding rules
 - Artifact templates
 - Review rules
 - Security rules
 - Quy tắc sử dụng dữ liệu
 - Quy tắc traceability
 - Quy tắc xử lý uncertainty
 - Điều kiện được phép sửa file
 - Điều kiện cấm thay đổi ngoài phạm vi

***00_context_index.md : Đây là file đầu tiên AI phải đọc. Không phải là đưa cho AI nhiều context nhất, mà là đưa context nhỏ nhất nhưng đủ căn cứ.
 
#1 Phase 1: 
  
 - Ticket / defect report / change request
 - Mục đích và bối cảnh nghiệp vụ
 - Hành vi mong muốn
 - Scope sơ bộ
 - Ràng buộc
 - Người xác nhận
 - Tài liệu nghiệp vụ liên quan
 - Dữ liệu hoặc màn hình mẫu nếu được phép
 - Các câu hỏi chưa có đáp án

***Không cần mọi nội dung đều đã rõ. Tuy nhiên, mỗi nội dung phải được phân loại (đã rõ, chưa rõ, chưa rõ nhưng phase sau đảm nhiệm,...)

#2 Phase 2: Điều tra hiện trạng và phân tích ảnh hưởng
Input chính
 - Kết quả sơ bộ của Phase 1
 - Search keywords
 - Source repository
 - Database definition
 - Related documents
 - Current logs / errors
 - Existing tests
 - Kiến trúc hệ thống
 - Quy tắc dự án
 - Danh sách giả thuyết cần kiểm chứng
 - Cải tiến quan trọng

***Phase 1 và Phase 2 nên chạy theo vòng lặp:

***Việc điều tra không nên chỉ thực hiện một lần. Khi xuất hiện yêu cầu điều tra mới, prompt cần được tinh chỉnh và chạy lại để cải thiện độ chính xác.

#3 Phase 3: Thiết kế và kế hoạch thực hiện
 - Input chính
 - Yêu cầu đã xác minh
 - Acceptance criteria
 - Current behavior
 - Required behavior
 - Call flow
 - Source / DB map
 - Impact analysis
 - Risk list
 - Coding / SQL conventions
 - Constraints
 - Conflict decisions
 - Open questions đã được xử lý hoặc có phương án

***Điểm kiểm tra quan trọng: Coding agent có thể thực hiện mà không phải tự diễn giải lại yêu cầu hay không?

#4 Phase 4: Thực hiện và kiểm chứng
 - Input chính
 - Logical design
 - Physical design
 - File-level change plan
 - SQL change plan
 - Test policy
 - Acceptance criteria
 - Regression scope
 - Test data rules
 - Build commands
 - Test commands
 - Evidence rules
 - Allowed / prohibited files
 - Rollback guidance


#5 Phase 5: Review cuối và đồng bộ tài liệu
 - Input chính
 - Final diff
 - Source code và SQL cuối cùng
 - Design documents
 - Acceptance criteria
 - Test cases
 - Test results
 - Evidence
 - Defect history
 - Known limitations
 - Release rules
 - Traceability matrix
 
***Điểm quan trọng nhất ở đây là evidence-based review, không chỉ kiểm tra “build thành công”. Trong tài liệu nội bộ, nhóm đã yêu cầu bằng chứng phải thể hiện điều gì thay đổi trước và sau test, thay vì chỉ lưu kết quả “passed”.

#6 Further research
*Context Engineering for Large Codebases
 Tập trung vào việc chọn đúng code, tài liệu và trạng thái hệ thống cho từng task.

*Requirements-to-Code Traceability
 Liên kết yêu cầu → thiết kế → file/method/SQL → test → evidence.

*Document-Code Consistency / Documentation Drift
 Phát hiện tài liệu cũ, mâu thuẫn hoặc không còn phù hợp với code.

*Hybrid Retrieval: Code + Documents + Knowledge Graph
 Kết hợp MCP, lexical search, semantic search và dependency graph thay vì chỉ vector search.

*Context Quality Metrics
 Đánh giá context trước khi cho phép AI tạo thiết kế hoặc sửa code.

