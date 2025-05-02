using Microsoft.AspNetCore.Mvc;
using SilkRoute.Demo.Shared.Contracts;
using SilkRoute.Demo.Shared.Models.Dto;
using SilkRoute.Demo.Shared.Models.RequestSnapshotting;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting;
using SilkRoute.Demo.TestMicroservice.TestFilesProviding;

namespace SilkRoute.Demo.TestMicroservice.Controllers;

[ApiController]
public sealed class TestController : ControllerBase, ITestMicroservice
{
    private readonly IRequestSnapshotBuilder _requestSnapshotBuilder;
    private readonly ITestFileProvider _testFileProvider;

    public TestController(
        IRequestSnapshotBuilder requestSnapshotBuilder,
        ITestFileProvider testFileProvider)
    {
        _requestSnapshotBuilder = requestSnapshotBuilder;
        _testFileProvider = testFileProvider;
    }

    #region Request Parameter Binding

    #region Query

    [HttpGet("api/test/bind/query/contains-primitives")]
    public Task<RequestSnapshot> Query_Contains_Primitives([FromQuery] int id,
        [FromQuery] bool flag,
        [FromQuery] string name)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-primitive-with-name")]
    public Task<RequestSnapshot> Query_Contains_Primitive_WithName([FromQuery(Name = "q")] string query)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-primitive-collections")]
    public Task<RequestSnapshot> Query_Contains_Primitive_Collections([FromQuery] int[] ids,
        [FromQuery] List<string> tags)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-primitive-dictionary")]
    public Task<RequestSnapshot> Query_Contains_Primitive_Dictionary([FromQuery] Dictionary<string, string> values)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-complexdto")]
    public Task<RequestSnapshot> Query_Contains_ComplexDto([FromQuery] ComplexDto dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-nesteddto")]
    public Task<RequestSnapshot> Query_Contains_NestedDto([FromQuery] NestedDto dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-complexdto-and-primitive")]
    public Task<RequestSnapshot> Query_Contains_ComplexDto_And_Primitive([FromQuery] ComplexDto dto, [FromQuery] int id)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-nullables")]
    public Task<RequestSnapshot> Query_Contains_Nullables([FromQuery] int? id, [FromQuery] string name)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-stream")]
    public Task<RequestSnapshot> Query_Contains_Stream([FromQuery] Stream strem)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-bytes")]
    public Task<RequestSnapshot> Query_Contains_Bytes([FromQuery] byte[] bytes)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-dto-with-nested-stream")]
    public Task<RequestSnapshot> Query_Contains_DtoWithNestedStream([FromQuery] DtoWithNestedStream dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/query/contains-dto-with-nested-bytes")]
    public Task<RequestSnapshot> Query_Contains_DtoWithNestedBytes([FromQuery] DtoWithNestedBytes dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    #endregion

    #region Route and Header

    [HttpGet("api/test/bind/route/contains-primitives/{id:int}/{name:alpha}")]
    public Task<RequestSnapshot> Route_Contains_Primitives([FromRoute] int id, [FromRoute] string name)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/route/contains-primitive-with-name/{id:int}")]
    public Task<RequestSnapshot> Route_Contains_Primitive_WithName([FromRoute(Name = "id")] int routeId)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/route/contains-primitive-and-has-length-constraint/{id:length(15)}")]
    public Task<RequestSnapshot> Route_Contains_Primitive_And_Has_Length_Constraint([FromRoute] string id)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/route/contains-primitive-and-has-length-and-alpha-constraints/{id:alpha:length(15)}")]
    public Task<RequestSnapshot> Route_Contains_Primitive_And_Has_Length_And_Alpha_Constraints([FromRoute] string id)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/header/contains-primitive")]
    public Task<RequestSnapshot> Header_Contains_Primitive([FromHeader] string value)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet("api/test/bind/header/contains-primitive-with-name")]
    public Task<RequestSnapshot> Header_Contains_Primitive_WithName([FromHeader(Name = "X-Test")] string value)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    #endregion

    #region Body

    [HttpPost("api/test/bind/body/contains-complexdto")]
    public Task<RequestSnapshot> Body_Contains_ComplexDto([FromBody] ComplexDto dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/body/contains-primitive-collection")]
    public Task<RequestSnapshot> Body_Contains_Primitive_Collection([FromBody] List<string> values)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/body/contains-primitive-dictionary")]
    public Task<RequestSnapshot> Body_Contains_Primitive_Dictionary([FromBody] Dictionary<string, string> values)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }
    
    [HttpPost("api/test/bind/body/contains-primitive")]
    public Task<RequestSnapshot> Body_Contains_Primitive([FromBody] string text)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/body/contains-stream")]
    public Task<RequestSnapshot> Body_Contains_Stream([FromBody] Stream body)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/body/contains-bytes")]
    public Task<RequestSnapshot> Body_Contains_Bytes([FromBody] byte[] body)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/body/contains-dto-with-nested-stream")]
    public Task<RequestSnapshot> Body_Contains_DtoWithNestedStream([FromBody] DtoWithNestedStream dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/body/contains-dto-with-nested-bytes")]
    public Task<RequestSnapshot> Body_Contains_DtoWithNestedBytes([FromBody] DtoWithNestedBytes dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/body/contains-dto-with-nested-formdata")]
    public Task<RequestSnapshot> Body_Contains_DtoWithNestedFormData([FromBody] DtoWithNestedFormData dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    // [HttpPost("api/test/bind/invalid/two-bodies")]
    // public Task<RequestSnapshot> Two_Bodies([FromBody] ComplexDto a, [FromBody] ComplexDto b)
    // {
    //     return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    // }

    #endregion

    #region Form

    [HttpPost("api/test/bind/form/contains-primitives")]
    public Task<RequestSnapshot> Form_Contains_Primitives([FromForm] string name, [FromForm] int id)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/form/contains-primitive-with-name")]
    public Task<RequestSnapshot> Form_Contains_Primitive_WithName([FromForm(Name = "comment")] string text)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/form/contains-primitive-dictionary")]
    public Task<RequestSnapshot> Form_Contains_Primitive_Dictionary([FromForm] Dictionary<string, string> values)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/form/contains-primitive-collection")]
    public Task<RequestSnapshot> Form_Contains_Primitive_Collection([FromForm] List<string> values)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/form/contains-iformfile")]
    [Consumes("multipart/form-data")]
    public Task<RequestSnapshot> Form_Contains_IFormFile( /*[FromForm]*/ IFormFile file)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/form/contains-iformfile-and-primitives")]
    public Task<RequestSnapshot> Form_Contains_IFormFile_And_Primitives( /*[FromForm]*/ IFormFile file,
        [FromForm] string comment)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }
    
    [HttpPost("api/test/bind/form/contains-iformfile-and-primitive-dictionary")]
    public Task<RequestSnapshot> Form_Contains_IFormFile_And_Primitive_Dictionary(
        /*[FromForm]*/ IFormFile file,
        [FromForm] Dictionary<string, string> values)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/form/contains-iformfile-collection")]
    public Task<RequestSnapshot> Form_Contains_IFormFile_Collection([FromForm] List<IFormFile> files)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/form/contains-iformfilecollection")]
    public Task<RequestSnapshot> Form_Contains_IFormFileCollection([FromForm] IFormFileCollection files)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/form/contains-iformfilecollection-and-primitives")]
    public Task<RequestSnapshot> Form_Contains_IFormFileCollection_And_Primitives([FromForm] IFormFileCollection files,
        [FromForm] string comment)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/form/contains-dto-with-nested-formdata")]
    public Task<RequestSnapshot> Form_Contains_DtoWithNestedFormData([FromForm] DtoWithNestedFormData dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    #endregion

    #region Body and Form combining (invalid-ish)

    [HttpPost("api/test/bind/invalid/body-complexdto-and-form-primitive")]
    public Task<RequestSnapshot> BodyComplexDto_And_FormPrimitive([FromBody] ComplexDto dto,
        [FromForm] string formValue)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/invalid/body-complexdto-and-iformfile")]
    public Task<RequestSnapshot> BodyComplexDto_And_IFormFile([FromBody] ComplexDto dto, IFormFile file)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    #endregion

    #region No Attributes

    // [HttpPost("api/test/bind/noattr/two-complexdtos/order-1-2")]
    // public Task<RequestSnapshot> NoAttr_TwoComplexDtos_Order_1_2(ComplexDto dto1, ComplexDto dto2)
    // {
    //     return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    // }
    //
    // [HttpPost("api/test/bind/noattr/two-complexdtos/order-2-1")]
    // public Task<RequestSnapshot> NoAttr_TwoComplexDtos_Order_2_1(ComplexDto dto2, ComplexDto dto1)
    // {
    //     return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    // }
    //
    // [HttpPost("api/test/bind/noattr/complexdto-and-bytes")]
    // public Task<RequestSnapshot> NoAttr_ComplexDto_And_Bytes(ComplexDto dto, byte[] bytes)
    // {
    //     return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    // }
    //
    // [HttpPost("api/test/bind/noattr/bytes-and-complexdto")]
    // public Task<RequestSnapshot> NoAttr_Bytes_And_ComplexDto(byte[] bytes, ComplexDto dto)
    // {
    //     return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    // }
    //
    // [HttpPost("api/test/bind/noattr/complexdto-and-stream")]
    // public Task<RequestSnapshot> NoAttr_ComplexDto_And_Stream(ComplexDto dto, Stream stream)
    // {
    //     return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    // }
    //
    // [HttpPost("api/test/bind/noattr/stream-and-complexdto")]
    // public Task<RequestSnapshot> NoAttr_Stream_And_ComplexDto(Stream stream, ComplexDto dto)
    // {
    //     return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    // }

    [HttpPost("api/test/bind/noattr/primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
    public Task<RequestSnapshot> NoAttr_PrimitiveAlsoInRouteTemplate_Primitive_And_ComplexDto(int id,
        string q,
        ComplexDto dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/noattr/complexdto-and-primitive-also-in-route-template-and-primitive/{id:int}")]
    public Task<RequestSnapshot> NoAttr_ComplexDto_And_PrimitiveAlsoInRouteTemplate_And_Primitive(ComplexDto dto,
        int id,
        string q)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/noattr/iformfile-and-primitive")]
    public Task<RequestSnapshot> NoAttr_IFormFile_And_Primitive(IFormFile file, string comment)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    #endregion

    #region Mixed different attributes

    [HttpPost(
        "api/test/bind/allattrs/route-primitive-also-in-route-template-and-query-primitive-and-body-complexdto/{id:int}")]
    public Task<RequestSnapshot> AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_BodyComplexDto(
        [FromRoute] int id,
        [FromQuery] string q,
        [FromBody] ComplexDto dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost(
        "api/test/bind/allattrs/route-primitive-also-in-route-template-and-header-primitive-and-body-complexdto/{id:int}")]
    public Task<RequestSnapshot> AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_HeaderPrimitive_And_BodyComplexDto(
        [FromRoute] int id,
        [FromHeader(Name = "X-Trace")] string traceId,
        [FromBody] ComplexDto dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpGet(
        "api/test/bind/allattrs/route-primitive-also-in-route-template-and-query-primitive-and-header-primitive/{id:int}")]
    public Task<RequestSnapshot> AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_HeaderPrimitive(
        [FromRoute] int id,
        [FromQuery] string q,
        [FromHeader(Name = "X-Test")] string header)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/allattrs/form-primitive-and-header-primitive")]
    public Task<RequestSnapshot> AllAttrs_FormPrimitive_And_HeaderPrimitive([FromForm] string comment,
        [FromHeader(Name = "X-Trace")] string traceId)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost(
        "api/test/bind/allattrs/route-primitive-and-query-primitive-and-header-primitive-and-form-primitive-with-names/{id:int}")]
    public Task<RequestSnapshot>
        AllAttrs_RoutePrimitive_And_QueryPrimitive_And_HeaderPrimitive_And_FormPrimitive_WithNames(
            [FromRoute(Name = "id")] int routeId,
            [FromQuery(Name = "q")] string query,
            [FromHeader(Name = "X-Trace")] string traceId,
            [FromForm(Name = "comment")] string comment)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    #endregion

    #region Mixed different attributes and no attributes

    [HttpPost("api/test/bind/mixed/route-primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
    public Task<RequestSnapshot> Mixed_RoutePrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto(
        [FromRoute] int id,
        string q,
        ComplexDto dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/mixed/primitive-also-in-route-template-and-query-primitive-and-complexdto/{id:int}")]
    public Task<RequestSnapshot> Mixed_PrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_ComplexDto(int id,
        [FromQuery] string q,
        ComplexDto dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/mixed/query-complexdto-and-complexdto")]
    public Task<RequestSnapshot> Mixed_QueryComplexDto_And_ComplexDto([FromQuery] ComplexDto dtoQuery,
        ComplexDto dtoBody)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/mixed/body-complexdto-and-primitives")]
    public Task<RequestSnapshot> Mixed_BodyComplexDto_And_Primitives([FromBody] ComplexDto dto, int page, string tag)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    // [HttpPost("api/test/bind/mixed/body-complexdto-and-complexdto")]
    // public Task<RequestSnapshot> Mixed_BodyComplexDto_And_ComplexDto([FromBody] ComplexDto dto1, ComplexDto dto2)
    // {
    //     return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    // }

    [HttpPost("api/test/bind/mixed/iformfile-and-form-primitive")]
    public Task<RequestSnapshot> Mixed_IFormFile_And_FormPrimitive(IFormFile file, [FromForm] string comment)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    [HttpPost("api/test/bind/mixed/query-primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
    public Task<RequestSnapshot> Mixed_QueryPrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto(
        [FromQuery] int id,
        string q,
        ComplexDto dto)
    {
        return _requestSnapshotBuilder.BuildAsync(HttpContext, HttpContext.RequestAborted);
    }

    #endregion

    #endregion

    #region Response Result Building

    #region Domain return types

    [HttpGet("api/test/return/domain/void")]
    public void Domain_Void()
    {
    }

    [HttpGet("api/test/return/domain/int")]
    public int Domain_Int()
    {
        return 123;
    }

    [HttpGet("api/test/return/domain/string")]
    public string Domain_String()
    {
        return "hello from Domain_String";
    }

    [HttpGet("api/test/return/domain/complexdto")]
    public ComplexDto Domain_ComplexDto()
    {
        return new ComplexDto
        {
            Id = 1,
            Name = "domain"
        };
    }

    [HttpGet("api/test/return/domain/bytes")]
    public byte[] Domain_Bytes()
    {
        return _testFileProvider.ReadTestPdfBytes();
    }

    [HttpGet("api/test/return/domain/stream")]
    public Stream Domain_Stream()
    {
        return _testFileProvider.OpenTestPdfStream();
    }

    [HttpGet("api/test/return/domain/task")]
    public Task Domain_Task()
    {
        return Task.CompletedTask;
    }

    [HttpGet("api/test/return/domain/task-int")]
    public Task<int> Domain_Task_Int()
    {
        return Task.FromResult(456);
    }

    [HttpGet("api/test/return/domain/task-string")]
    public Task<string> Domain_Task_String()
    {
        return Task.FromResult("hello from Domain_Task_String");
    }

    [HttpGet("api/test/return/domain/task-complexdto")]
    public Task<ComplexDto> Domain_Task_ComplexDto()
    {
        return Task.FromResult(new ComplexDto
        {
            Id = 2,
            Name = "domain-task"
        });
    }

    [HttpGet("api/test/return/domain/task-bytes")]
    public Task<byte[]> Domain_Task_Bytes()
    {
        return Task.FromResult(_testFileProvider.ReadTestPdfBytes());
    }

    [HttpGet("api/test/return/domain/task-stream")]
    public Task<Stream> Domain_Task_Stream()
    {
        return Task.FromResult(_testFileProvider.OpenTestPdfStream());
    }

    #endregion

    #region ActionResult<T> return types

    [HttpGet("api/test/return/actionresultt/int")]
    public ActionResult<int> ActionResultT_Int()
    {
        return 777;
    }

    [HttpGet("api/test/return/actionresultt/complexdto")]
    public ActionResult<ComplexDto> ActionResultT_ComplexDto()
    {
        return new ComplexDto
        {
            Id = 3,
            Name = "ar"
        };
    }

    [HttpGet("api/test/return/actionresultt/string")]
    public ActionResult<string> ActionResultT_String()
    {
        return "hello from ActionResultT_String";
    }

    [HttpGet("api/test/return/actionresultt/bytes")]
    public ActionResult<byte[]> ActionResultT_Bytes()
    {
        return _testFileProvider.ReadTestPdfBytes();
    }

    [HttpGet("api/test/return/actionresultt/stream")]
    public ActionResult<Stream> ActionResultT_Stream()
    {
        return _testFileProvider.OpenTestPdfStream();
    }

    #endregion

    #region Concrete ActionResult implementations return types

    [HttpGet("api/test/return/concrete-actionresult/contentresult-string")]
    public ContentResult ContentResult_String()
    {
        return new ContentResult
        {
            Content = "hello from ContentResult",
            ContentType = "text/plain",
            StatusCode = 200
        };
    }

    [HttpGet("api/test/return/concrete-actionresult/objectresult-complexdto")]
    public ObjectResult ObjectResult_ComplexDto()
    {
        return new ObjectResult(new ComplexDto
        {
            Id = 4,
            Name = "objectresult"
        })
        {
            StatusCode = 200
        };
    }

    [HttpGet("api/test/return/concrete-actionresult/okobjectresult-complexdto")]
    public OkObjectResult OkObjectResult_ComplexDto()
    {
        return new OkObjectResult(new ComplexDto
        {
            Id = 4,
            Name = "okobjectresult"
        });
    }

    [HttpGet("api/test/return/concrete-actionresult/jsonresult-complexdto")]
    public JsonResult JsonResult_ComplexDto()
    {
        return new JsonResult(new ComplexDto
        {
            Id = 4,
            Name = "jsonresult"
        });
    }

    [HttpGet("api/test/return/concrete-actionresult/statuscoderesult-418")]
    public StatusCodeResult StatusCodeResult_418()
    {
        return new StatusCodeResult(418);
    }

    [HttpGet("api/test/return/concrete-actionresult/okresult-200")]
    public OkResult OkResult_200()
    {
        return new OkResult();
    }

    [HttpGet("api/test/return/concrete-actionresult/filecontentresult-bytes")]
    public FileContentResult FileContentResult_Bytes()
    {
        var bytes = _testFileProvider.ReadTestPdfBytes();
        return new FileContentResult(bytes, "application/pdf")
        {
            FileDownloadName = "test.pdf"
        };
    }

    [HttpGet("api/test/return/concrete-actionresult/filestreamresult-stream")]
    public FileStreamResult FileStreamResult_Stream()
    {
        var stream = _testFileProvider.OpenTestPdfStream();
        return new FileStreamResult(stream, "application/pdf")
        {
            FileDownloadName = "test.pdf"
        };
    }

    #endregion

    #region Abstract and not explicitly supported ActionResults return types

    [HttpGet("api/test/return/abstract-and-default-actionresult/iactionresult-objectresult-complexdto")]
    public IActionResult IActionResult_ObjectResult_ComplexDto()
    {
        return new ObjectResult(new ComplexDto
        {
            Id = 5,
            Name = "iactionresult-objectresult"
        })
        {
            StatusCode = 200
        };
    }

    [HttpGet("api/test/return/abstract-and-default-actionresult/actionresult-contentresult-string")]
    public ActionResult ActionResult_ContentResult_String()
    {
        return new ContentResult
            { Content = "hello from ActionResult(ContentResult)", ContentType = "text/plain", StatusCode = 200 };
    }

    [HttpGet("api/test/return/abstract-and-default-actionresult/iactionresult-filecontentresult-bytes")]
    public IActionResult IActionResult_FileContentResult_Bytes()
    {
        return new FileContentResult(_testFileProvider.ReadTestPdfBytes(), "application/pdf")
            { FileDownloadName = "test.pdf" };
    }

    [HttpGet("api/test/return/abstract-and-default-actionresult/actionresult-filestreamresult-stream")]
    public ActionResult ActionResult_FileStreamResult_Stream()
    {
        return new FileStreamResult(_testFileProvider.OpenTestPdfStream(), "application/pdf")
            { FileDownloadName = "test.pdf" };
    }

    [HttpGet("api/test/return/abstract-and-default-actionresult/iactionresult-statuscoderesult-418")]
    public IActionResult IActionResult_StatusCodeResult_418()
    {
        return new StatusCodeResult(418);
    }

    [HttpGet("api/test/return/abstract-and-default-actionresult/redirectresult")]
    public RedirectResult RedirectResult()
    {
        return new RedirectResult("/somewhere-else", false);
    }

    [HttpGet("api/test/return/abstract-and-default-actionresult/forbidresult")]
    public ForbidResult ForbidResult()
    {
        return new ForbidResult();
    }

    [HttpGet("api/test/return/abstract-and-default-actionresult/challengeresult")]
    public ChallengeResult ChallengeResult()
    {
        return new ChallengeResult();
    }

    #endregion

    #endregion
}