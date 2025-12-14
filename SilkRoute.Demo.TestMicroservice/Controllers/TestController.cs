using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Demo.Shared.Contracts;
using SilkRoute.Demo.Shared.Models;

namespace SilkRoute.Demo.TestMicroservice.Controllers
{
    [ApiController]
    public sealed class TestController : ControllerBase, ITestMicroservice
    {
        #region Request Parameter Binding

        #region Query

        [HttpGet("api/test/bind/query/contains-primitives")]
        public Task<RequestSnapshot> Query_Contains_Primitives([FromQuery] int id, [FromQuery] bool flag, [FromQuery] string name)
            => SnapshotAsync(new { id, flag, name });

        [HttpGet("api/test/bind/query/contains-primitive-collections")]
        public Task<RequestSnapshot> Query_Contains_Primitive_Collections([FromQuery] int[] ids, [FromQuery] List<string> tags)
            => SnapshotAsync(new { ids, tags });

        [HttpGet("api/test/bind/query/contains-complexdto")]
        public Task<RequestSnapshot> Query_Contains_ComplexDto([FromQuery] ComplexDto dto)
            => SnapshotAsync(new { dto });

        [HttpGet("api/test/bind/query/contains-nesteddto")]
        public Task<RequestSnapshot> Query_Contains_NestedDto([FromQuery] NestedDto dto)
            => SnapshotAsync(new { dto });

        [HttpGet("api/test/bind/query/contains-complexdto-and-primitive")]
        public Task<RequestSnapshot> Query_Contains_ComplexDto_And_Primitive([FromQuery] ComplexDto dto, [FromQuery] int id)
            => SnapshotAsync(new { dto, id });

        [HttpGet("api/test/bind/query/contains-nullables")]
        public Task<RequestSnapshot> Query_Contains_Nullables([FromQuery] int? id, [FromQuery] string? name)
            => SnapshotAsync(new { id, name });

        [HttpGet("api/test/bind/query/contains-stream")]
        public Task<RequestSnapshot> Query_Contains_Stream([FromQuery] DtoWithNestedStream dto)
            => SnapshotAsync(new { dto = new { dto.Id, dto.Name, Data = "(Stream in DTO - should be blocked by client before reaching server)" } });

        [HttpGet("api/test/bind/query/contains-bytes")]
        public Task<RequestSnapshot> Query_Contains_Bytes([FromQuery] DtoWithNestedBytes dto)
            => SnapshotAsync(new { dto = new { dto.Id, dto.Name, DataLength = dto.Data?.Length } });

        #endregion

        #region Route and Header

        [HttpGet("api/test/bind/route/contains-primitive/{id:int}")]
        public Task<RequestSnapshot> Route_Contains_Primitive([FromRoute] int id)
            => SnapshotAsync(new { id });

        [HttpGet("api/test/bind/header/contains-primitive")]
        public Task<RequestSnapshot> Header_Contains_Primitive([FromHeader(Name = "X-Test")] string value)
            => SnapshotAsync(new { header = new { X_Test = value } });

        #endregion

        #region Body

        [HttpPost("api/test/bind/body/contains-complexdto")]
        public Task<RequestSnapshot> Body_Contains_ComplexDto([FromBody] ComplexDto dto)
            => SnapshotAsync(parameters: new { dto }, bodyValue: dto);

        [HttpPost("api/test/bind/body/contains-primitive")]
        public Task<RequestSnapshot> Body_Contains_Primitive([FromBody] string text)
            => SnapshotAsync(parameters: new { text }, bodyValue: text);

        [HttpPost("api/test/bind/body/contains-stream")]
        public Task<RequestSnapshot> Body_Contains_Stream([FromBody] Stream body)
            => SnapshotAsync(parameters: new { body = "(Stream)" }, bodyValue: body);

        [HttpPost("api/test/bind/body/contains-bytes")]
        public Task<RequestSnapshot> Body_Contains_Bytes([FromBody] byte[] body)
            => SnapshotAsync(parameters: new { bytesLength = body?.Length }, bodyValue: body);

        [HttpPost("api/test/bind/body/contains-dto-with-nested-stream")]
        public Task<RequestSnapshot> Body_Contains_DtoWithNestedStream([FromBody] DtoWithNestedStream dto)
            => SnapshotAsync(parameters: new { dto = new { dto.Id, dto.Name, Data = "(nested Stream)" } }, bodyValue: dto);

        [HttpPost("api/test/bind/body/contains-dto-with-nested-bytes")]
        public Task<RequestSnapshot> Body_Contains_DtoWithNestedBytes([FromBody] DtoWithNestedBytes dto)
            => SnapshotAsync(parameters: new { dto = new { dto.Id, dto.Name, DataLength = dto.Data?.Length } }, bodyValue: dto);

        [HttpPost("api/test/bind/body/contains-dto-with-nested-formdata")]
        public Task<RequestSnapshot> Body_Contains_DtoWithNestedFormData([FromBody] DtoWithNestedFormData dto)
            => SnapshotAsync(parameters: new { dto = new { dto.Id, dto.Name, File = "(IFormFile nested in DTO)" } }, bodyValue: dto);

        // фреймворк зазвичай не дасть цьому нормально жити, але метод для тестів/контракту є
        [HttpPost("api/test/bind/invalid/two-bodies")]
        public Task<RequestSnapshot> Two_Bodies([FromBody] ComplexDto a, [FromBody] ComplexDto b)
            => SnapshotAsync(parameters: new { a, b }, bodyValue: new { a, b });

        #endregion

        #region Form

        [HttpPost("api/test/bind/form/contains-primitives")]
        public Task<RequestSnapshot> Form_Contains_Primitives([FromForm] string name, [FromForm] int id)
            => SnapshotAsync(parameters: new { name, id }, includeForm: true);

        [HttpPost("api/test/bind/form/contains-iformfile")]
        public Task<RequestSnapshot> Form_Contains_IFormFile([FromForm] IFormFile file)
            => SnapshotAsync(parameters: new { file = new { file?.Name, file?.FileName, file?.ContentType, file?.Length } }, includeForm: true);

        [HttpPost("api/test/bind/form/contains-iformfile-and-primitives")]
        public Task<RequestSnapshot> Form_Contains_IFormFile_And_Primitives([FromForm] IFormFile file, [FromForm] string comment)
            => SnapshotAsync(parameters: new { comment, file = new { file?.Name, file?.FileName, file?.ContentType, file?.Length } }, includeForm: true);

        [HttpPost("api/test/bind/form/contains-iformfiles")]
        public Task<RequestSnapshot> Form_Contains_IFormFiles([FromForm] List<IFormFile> files)
            => SnapshotAsync(parameters: new
            {
                files = files?.Select(f => new { f.Name, f.FileName, f.ContentType, f.Length }).ToList()
            }, includeForm: true);

        #endregion

        #region Body and Form combining (invalid-ish)

        [HttpPost("api/test/bind/invalid/body-complexdto-and-form-primitive")]
        public Task<RequestSnapshot> BodyComplexDto_And_FormPrimitive([FromBody] ComplexDto dto, [FromForm] string formValue)
            => SnapshotAsync(parameters: new { dto, formValue }, bodyValue: dto, includeForm: true);

        [HttpPost("api/test/bind/invalid/body-complexdto-and-iformfile")]
        public Task<RequestSnapshot> BodyComplexDto_And_IFormFile([FromBody] ComplexDto dto, IFormFile file)
            => SnapshotAsync(parameters: new { dto, file = new { file?.Name, file?.FileName, file?.ContentType, file?.Length } }, bodyValue: dto, includeForm: true);

        #endregion

        #region No Attributes

        [HttpPost("api/test/bind/noattr/two-complexdtos/order-1-2")]
        public Task<RequestSnapshot> NoAttr_TwoComplexDtos_Order_1_2(ComplexDto dto1, ComplexDto dto2)
            => SnapshotAsync(parameters: new { dto1, dto2 }, bodyValue: dto1);

        [HttpPost("api/test/bind/noattr/two-complexdtos/order-2-1")]
        public Task<RequestSnapshot> NoAttr_TwoComplexDtos_Order_2_1(ComplexDto dto2, ComplexDto dto1)
            => SnapshotAsync(parameters: new { dto2, dto1 }, bodyValue: dto2);

        [HttpPost("api/test/bind/noattr/complexdto-and-bytes")]
        public Task<RequestSnapshot> NoAttr_ComplexDto_And_Bytes(ComplexDto dto, byte[] bytes)
            => SnapshotAsync(parameters: new { dto, bytesLength = bytes?.Length }, bodyValue: dto);

        [HttpPost("api/test/bind/noattr/bytes-and-complexdto")]
        public Task<RequestSnapshot> NoAttr_Bytes_And_ComplexDto(byte[] bytes, ComplexDto dto)
            => SnapshotAsync(parameters: new { bytesLength = bytes?.Length, dto }, bodyValue: bytes);

        [HttpPost("api/test/bind/noattr/complexdto-and-stream")]
        public Task<RequestSnapshot> NoAttr_ComplexDto_And_Stream(ComplexDto dto, Stream stream)
            => SnapshotAsync(parameters: new { dto, stream = "(Stream)" }, bodyValue: dto);

        [HttpPost("api/test/bind/noattr/stream-and-complexdto")]
        public Task<RequestSnapshot> NoAttr_Stream_And_ComplexDto(Stream stream, ComplexDto dto)
            => SnapshotAsync(parameters: new { stream = "(Stream)", dto }, bodyValue: stream);

        [HttpPost("api/test/bind/noattr/primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
        public Task<RequestSnapshot> NoAttr_PrimitiveAlsoInRouteTemplate_Primitive_And_ComplexDto(int id, string q, ComplexDto dto)
            => SnapshotAsync(parameters: new { id, q, dto }, bodyValue: dto);

        [HttpPost("api/test/bind/noattr/complexdto-and-primitive-also-in-route-template-and-primitive/{id:int}")]
        public Task<RequestSnapshot> NoAttr_ComplexDto_And_PrimitiveAlsoInRouteTemplate_And_Primitive(ComplexDto dto, int id, string q)
            => SnapshotAsync(parameters: new { dto, id, q }, bodyValue: dto);

        [HttpPost("api/test/bind/noattr/iformfile-and-primitive")]
        public Task<RequestSnapshot> NoAttr_IFormFile_And_Primitive(IFormFile file, string comment)
            => SnapshotAsync(parameters: new { comment, file = new { file?.Name, file?.FileName, file?.ContentType, file?.Length } }, includeForm: true);

        #endregion

        #region Mixed different attributes

        [HttpPost("api/test/bind/allattrs/route-primitive-also-in-route-template-and-query-primitive-and-body-complexdto/{id:int}")]
        public Task<RequestSnapshot> AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_BodyComplexDto([FromRoute] int id, [FromQuery] string q, [FromBody] ComplexDto dto)
            => SnapshotAsync(parameters: new { id, q, dto }, bodyValue: dto);

        [HttpPost("api/test/bind/allattrs/route-primitive-also-in-route-template-and-header-primitive-and-body-complexdto/{id:int}")]
        public Task<RequestSnapshot> AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_HeaderPrimitive_And_BodyComplexDto([FromRoute] int id, [FromHeader(Name = "X-Trace")] string traceId, [FromBody] ComplexDto dto)
            => SnapshotAsync(parameters: new { id, traceId, dto }, bodyValue: dto);

        [HttpGet("api/test/bind/allattrs/route-primitive-also-in-route-template-and-query-primitive-and-header-primitive/{id:int}")]
        public Task<RequestSnapshot> AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_HeaderPrimitive([FromRoute] int id, [FromQuery] string q, [FromHeader(Name = "X-Test")] string header)
            => SnapshotAsync(parameters: new { id, q, header });

        [HttpPost("api/test/bind/allattrs/form-primitive-and-header-primitive")]
        public Task<RequestSnapshot> AllAttrs_FormPrimitive_And_HeaderPrimitive([FromForm] string comment, [FromHeader(Name = "X-Trace")] string traceId)
            => SnapshotAsync(parameters: new { comment, traceId }, includeForm: true);

        #endregion

        #region Mixed different attributes and no attributes

        [HttpPost("api/test/bind/mixed/route-primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
        public Task<RequestSnapshot> Mixed_RoutePrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto([FromRoute] int id, string q, ComplexDto dto)
            => SnapshotAsync(parameters: new { id, q, dto }, bodyValue: dto);

        [HttpPost("api/test/bind/mixed/primitive-also-in-route-template-and-query-primitive-and-complexdto/{id:int}")]
        public Task<RequestSnapshot> Mixed_PrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_ComplexDto(int id, [FromQuery] string q, ComplexDto dto)
            => SnapshotAsync(parameters: new { id, q, dto }, bodyValue: dto);

        [HttpPost("api/test/bind/mixed/query-complexdto-and-complexdto")]
        public Task<RequestSnapshot> Mixed_QueryComplexDto_And_ComplexDto([FromQuery] ComplexDto dtoQuery, ComplexDto dtoBody)
            => SnapshotAsync(parameters: new { dtoQuery, dtoBody }, bodyValue: dtoBody);

        [HttpPost("api/test/bind/mixed/body-complexdto-and-primitives")]
        public Task<RequestSnapshot> Mixed_BodyComplexDto_And_Primitives([FromBody] ComplexDto dto, int page, string tag)
            => SnapshotAsync(parameters: new { dto, page, tag }, bodyValue: dto);

        [HttpPost("api/test/bind/mixed/body-complexdto-and-complexdto")]
        public Task<RequestSnapshot> Mixed_BodyComplexDto_And_ComplexDto([FromBody] ComplexDto dto1, ComplexDto dto2)
            => SnapshotAsync(parameters: new { dto1, dto2 }, bodyValue: dto1);

        [HttpPost("api/test/bind/mixed/iformfile-and-form-primitive")]
        public Task<RequestSnapshot> Mixed_IFormFile_And_FormPrimitive(IFormFile file, [FromForm] string comment)
            => SnapshotAsync(parameters: new { comment, file = new { file?.Name, file?.FileName, file?.ContentType, file?.Length } }, includeForm: true);

        [HttpPost("api/test/bind/mixed/query-primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
        public Task<RequestSnapshot> Mixed_QueryPrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto([FromQuery] int id, string q, ComplexDto dto)
            => SnapshotAsync(parameters: new { id, q, dto }, bodyValue: dto);

        #endregion

        #endregion

        #region Response Result Building

        #region Domain return types

        [HttpGet("api/test/return/domain/void")]
        public void Domain_Void() { }

        [HttpGet("api/test/return/domain/int")]
        public int Domain_Int() => 123;

        [HttpGet("api/test/return/domain/string")]
        public string Domain_String() => "hello from Domain_String";

        [HttpGet("api/test/return/domain/complexdto")]
        public ComplexDto Domain_ComplexDto() => new ComplexDto(1, "domain");

        [HttpGet("api/test/return/domain/bytes")]
        public byte[] Domain_Bytes() => ReadTestPdfBytes();

        [HttpGet("api/test/return/domain/stream")]
        public Stream Domain_Stream() => OpenTestPdfStream();

        [HttpGet("api/test/return/domain/task")]
        public Task Domain_Task() => Task.CompletedTask;

        [HttpGet("api/test/return/domain/task-int")]
        public Task<int> Domain_Task_Int() => Task.FromResult(456);

        [HttpGet("api/test/return/domain/task-string")]
        public Task<string> Domain_Task_String() => Task.FromResult("hello from Domain_Task_String");

        [HttpGet("api/test/return/domain/task-complexdto")]
        public Task<ComplexDto> Domain_Task_ComplexDto() => Task.FromResult(new ComplexDto(2, "domain-task"));

        [HttpGet("api/test/return/domain/task-bytes")]
        public Task<byte[]> Domain_Task_Bytes() => Task.FromResult(ReadTestPdfBytes());

        [HttpGet("api/test/return/domain/task-stream")]
        public Task<Stream> Domain_Task_Stream() => Task.FromResult<Stream>(OpenTestPdfStream());

        #endregion

        #region ActionResult<T> return types

        [HttpGet("api/test/return/actionresultt/int")]
        public ActionResult<int> ActionResultT_Int() => 777;

        [HttpGet("api/test/return/actionresultt/complexdto")]
        public ActionResult<ComplexDto> ActionResultT_ComplexDto() => new ComplexDto(3, "ar");

        [HttpGet("api/test/return/actionresultt/string")]
        public ActionResult<string> ActionResultT_String() => "hello from ActionResultT_String";

        [HttpGet("api/test/return/actionresultt/bytes")]
        public ActionResult<byte[]> ActionResultT_Bytes() => ReadTestPdfBytes();

        [HttpGet("api/test/return/actionresultt/stream")]
        public ActionResult<Stream> ActionResultT_Stream() => OpenTestPdfStream();

        #endregion

        #region Concrete ActionResult implementations return types

        [HttpGet("api/test/return/concrete-actionresult/contentresult-string")]
        public ContentResult ContentResult_String()
            => new ContentResult
            {
                Content = "hello from ContentResult",
                ContentType = "text/plain",
                StatusCode = 200
            };

        [HttpGet("api/test/return/concrete-actionresult/objectresult-complexdto")]
        public ObjectResult ObjectResult_ComplexDto()
            => new ObjectResult(new ComplexDto(4, "objectresult")) { StatusCode = 200 };

        [HttpGet("api/test/return/concrete-actionresult/statuscoderesult-418")]
        public StatusCodeResult StatusCodeResult_418()
            => new StatusCodeResult(418);

        [HttpGet("api/test/return/concrete-actionresult/filecontentresult-bytes")]
        public FileContentResult FileContentResult_Bytes()
        {
            var bytes = ReadTestPdfBytes();
            return new FileContentResult(bytes, "application/pdf")
            {
                FileDownloadName = "test.pdf"
            };
        }

        [HttpGet("api/test/return/concrete-actionresult/filestreamresult-stream")]
        public FileStreamResult FileStreamResult_Stream()
        {
            var stream = OpenTestPdfStream();
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = "test.pdf"
            };
        }

        #endregion

        #region Abstract and not explicitly supported ActionResults return types

        [HttpGet("api/test/return/abstract-and-default-actionresult/iactionresult-objectresult-complexdto")]
        public IActionResult IActionResult_ObjectResult_ComplexDto()
            => new ObjectResult(new ComplexDto(5, "iactionresult-objectresult")) { StatusCode = 200 };

        [HttpGet("api/test/return/abstract-and-default-actionresult/actionresult-contentresult-string")]
        public ActionResult ActionResult_ContentResult_String()
            => new ContentResult { Content = "hello from ActionResult(ContentResult)", ContentType = "text/plain", StatusCode = 200 };

        [HttpGet("api/test/return/abstract-and-default-actionresult/iactionresult-filecontentresult-bytes")]
        public IActionResult IActionResult_FileContentResult_Bytes()
            => new FileContentResult(ReadTestPdfBytes(), "application/pdf") { FileDownloadName = "test.pdf" };

        [HttpGet("api/test/return/abstract-and-default-actionresult/actionresult-filestreamresult-stream")]
        public ActionResult ActionResult_FileStreamResult_Stream()
            => new FileStreamResult(OpenTestPdfStream(), "application/pdf") { FileDownloadName = "test.pdf" };

        [HttpGet("api/test/return/abstract-and-default-actionresult/iactionresult-statuscoderesult-418")]
        public IActionResult IActionResult_StatusCodeResult_418()
            => new StatusCodeResult(418);

        [HttpGet("api/test/return/abstract-and-default-actionresult/redirectresult")]
        public RedirectResult RedirectResult()
            => new RedirectResult("/somewhere-else", permanent: false);

        [HttpGet("api/test/return/abstract-and-default-actionresult/forbidresult")]
        public ForbidResult ForbidResult()
            => new ForbidResult();

        [HttpGet("api/test/return/abstract-and-default-actionresult/challengeresult")]
        public ChallengeResult ChallengeResult()
            => new ChallengeResult();

        #endregion

        #endregion


        #region Request snapshot helper

        private async Task<RequestSnapshot> SnapshotAsync(object? parameters, object? bodyValue = null, bool includeForm = false)
        {
            var baseSnapshot = CreateBaseSnapshot();

            RequestSnapshot.BodySnapshot? body = null;
            if (bodyValue != null)
            {
                body = await CreateBodySnapshotFromValueAsync(bodyValue);
            }
            else if (parameters != null)
            {
                body = new RequestSnapshot.BodySnapshot
                {
                    ContentType = Request.ContentType,
                    Length = Request.ContentLength,
                    Kind = "parameters",
                    Text = RequestSnapshot.ToJson(parameters)
                };
            }

            RequestSnapshot.FormSnapshot? form = null;
            if (includeForm)
            {
                form = await TryCreateFormSnapshotAsync();
            }

            return new RequestSnapshot
            {
                TimestampUtc = baseSnapshot.TimestampUtc,
                HttpMethod = baseSnapshot.HttpMethod,
                Path = baseSnapshot.Path,
                RoutePattern = baseSnapshot.RoutePattern,
                RouteValues = baseSnapshot.RouteValues,
                Query = baseSnapshot.Query,
                Headers = baseSnapshot.Headers,
                Body = body,
                Form = form
            };
        }

        private RequestSnapshot CreateBaseSnapshot()
        {
            var endpoint = HttpContext.GetEndpoint();
            var routeEndpoint = endpoint as RouteEndpoint;

            var routeValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in RouteData.Values)
                routeValues[kv.Key] = kv.Value?.ToString();

            var query = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in Request.Query)
                query[kv.Key] = kv.Value.ToString();

            var headers = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in Request.Headers)
            {
                if (kv.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                    continue;

                headers[kv.Key] = kv.Value.ToString();
            }

            return new RequestSnapshot
            {
                TimestampUtc = DateTimeOffset.UtcNow,
                HttpMethod = Request.Method,
                Path = $"{Request.Path}{Request.QueryString}",
                RoutePattern = routeEndpoint?.RoutePattern?.RawText,
                RouteValues = routeValues,
                Query = query,
                Headers = headers
            };
        }

        private async Task<RequestSnapshot.BodySnapshot> CreateBodySnapshotFromValueAsync(object bodyValue)
        {
            var ct = Request.ContentType;
            var len = Request.ContentLength;

            switch (bodyValue)
            {
                case string s:
                    return new RequestSnapshot.BodySnapshot
                    {
                        ContentType = ct,
                        Length = len,
                        Kind = "string",
                        Text = s
                    };

                case byte[] bytes:
                    return new RequestSnapshot.BodySnapshot
                    {
                        ContentType = ct,
                        Length = len,
                        Kind = "bytes",
                        BytesLength = bytes.Length,
                        Base64 = Convert.ToBase64String(bytes)
                    };

                case Stream stream:
                    {
                        using var ms = new MemoryStream();
                        stream.CopyTo(ms);

                        var bytes = ms.ToArray();
                        return new RequestSnapshot.BodySnapshot
                        {
                            ContentType = ct,
                            Length = len,
                            Kind = "stream",
                            BytesLength = bytes.Length,
                            Base64 = Convert.ToBase64String(bytes)
                        };
                    }

                default:
                    return new RequestSnapshot.BodySnapshot
                    {
                        ContentType = ct,
                        Length = len,
                        Kind = "json",
                        Text = RequestSnapshot.ToJson(bodyValue)
                    };
            }
        }

        private async Task<RequestSnapshot.FormSnapshot?> TryCreateFormSnapshotAsync()
        {
            if (!Request.HasFormContentType)
                return null;

            IFormCollection form;
            try
            {
                form = await Request.ReadFormAsync();
            }
            catch
            {
                return null;
            }

            var fields = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in form)
                fields[kv.Key] = kv.Value.ToString();

            var files = new List<RequestSnapshot.FormFileSnapshot>();
            foreach (var f in form.Files)
            {
                files.Add(new RequestSnapshot.FormFileSnapshot
                {
                    Name = f.Name,
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    Length = f.Length
                });
            }

            return new RequestSnapshot.FormSnapshot
            {
                Fields = fields,
                Files = files
            };
        }

        private static string GetTestPdfPath()
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            return Path.Combine(desktop, "test.pdf");
        }

        private static byte[] ReadTestPdfBytes()
        {
            var path = GetTestPdfPath();
            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            return System.IO.File.ReadAllBytes(path);
        }

        private static Stream OpenTestPdfStream()
        {
            var path = GetTestPdfPath();
            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            return System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        #endregion
    }
}
