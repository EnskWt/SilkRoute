using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Demo.Shared.Models.Dto;
using SilkRoute.Demo.Shared.Models.RequestSnapshotting;
using SilkRoute.Public.Abstractions;

namespace SilkRoute.Demo.Shared.Contracts;

public interface ITestMicroservice : IMicroserviceClient
{
    #region Request Parameter Binding

    #region Query

    [HttpGet("api/test/bind/query/contains-primitives")]
    Task<RequestSnapshot> Query_Contains_Primitives([FromQuery] int id, [FromQuery] bool flag, [FromQuery] string name);

    [HttpGet("api/test/bind/query/contains-primitive-with-name")]
    Task<RequestSnapshot> Query_Contains_Primitive_WithName([FromQuery(Name = "q")] string query);

    [HttpGet("api/test/bind/query/contains-primitive-collections")]
    Task<RequestSnapshot> Query_Contains_Primitive_Collections([FromQuery] int[] ids, [FromQuery] List<string> tags);

    [HttpGet("api/test/bind/query/contains-primitive-dictionary")]
    Task<RequestSnapshot> Query_Contains_Primitive_Dictionary([FromQuery] Dictionary<string, string> values);

    [HttpGet("api/test/bind/query/contains-complexdto")]
    Task<RequestSnapshot> Query_Contains_ComplexDto([FromQuery] ComplexDto dto);

    [HttpGet("api/test/bind/query/contains-nesteddto")]
    Task<RequestSnapshot> Query_Contains_NestedDto([FromQuery] NestedDto dto);

    [HttpGet("api/test/bind/query/contains-complexdto-and-primitive")]
    Task<RequestSnapshot> Query_Contains_ComplexDto_And_Primitive([FromQuery] ComplexDto dto, [FromQuery] int id);

    [HttpGet("api/test/bind/query/contains-nullables")]
    Task<RequestSnapshot> Query_Contains_Nullables([FromQuery] int? id, [FromQuery] string name);

    [HttpGet("api/test/bind/query/contains-stream")]
    Task<RequestSnapshot> Query_Contains_Stream([FromQuery] Stream stream);

    [HttpGet("api/test/bind/query/contains-bytes")]
    Task<RequestSnapshot> Query_Contains_Bytes([FromQuery] byte[] bytes);

    [HttpGet("api/test/bind/query/contains-dto-with-nested-stream")]
    Task<RequestSnapshot> Query_Contains_DtoWithNestedStream([FromQuery] DtoWithNestedStream dto);

    [HttpGet("api/test/bind/query/contains-dto-with-nested-bytes")]
    Task<RequestSnapshot> Query_Contains_DtoWithNestedBytes([FromQuery] DtoWithNestedBytes dto);

    #endregion

    #region Route and Header

    [HttpGet("api/test/bind/route/contains-primitives/{id:int}/{name:alpha}")]
    Task<RequestSnapshot> Route_Contains_Primitives([FromRoute] int id, [FromRoute] string name);

    [HttpGet("api/test/bind/route/contains-primitive-with-name/{id:int}")]
    Task<RequestSnapshot> Route_Contains_Primitive_WithName([FromRoute(Name = "id")] int routeId);

    [HttpGet("api/test/bind/route/contains-primitive-and-has-length-constraint/{id:length(15)}")]
    Task<RequestSnapshot> Route_Contains_Primitive_And_Has_Length_Constraint([FromRoute] string id);

    [HttpGet("api/test/bind/route/contains-primitive-and-has-length-and-alpha-constraints/{id:alpha:length(15)}")]
    Task<RequestSnapshot> Route_Contains_Primitive_And_Has_Length_And_Alpha_Constraints([FromRoute] string id);

    [HttpGet("api/test/bind/header/contains-primitive")]
    Task<RequestSnapshot> Header_Contains_Primitive([FromHeader] string value);

    [HttpGet("api/test/bind/header/contains-primitive-with-name")]
    Task<RequestSnapshot> Header_Contains_Primitive_WithName([FromHeader(Name = "X-Test")] string value);

    #endregion

    #region Body

    [HttpPost("api/test/bind/body/contains-complexdto")]
    Task<RequestSnapshot> Body_Contains_ComplexDto([FromBody] ComplexDto dto);

    [HttpPost("api/test/bind/body/contains-primitive-collection")]
    Task<RequestSnapshot> Body_Contains_Primitive_Collection([FromBody] List<string> values);

    [HttpPost("api/test/bind/body/contains-primitive-dictionary")]
    Task<RequestSnapshot> Body_Contains_Primitive_Dictionary([FromBody] Dictionary<string, string> values);

    [HttpPost("api/test/bind/body/contains-primitive")]
    Task<RequestSnapshot> Body_Contains_Primitive([FromBody] string text);

    [HttpPost("api/test/bind/body/contains-stream")]
    Task<RequestSnapshot> Body_Contains_Stream([FromBody] Stream body);

    [HttpPost("api/test/bind/body/contains-bytes")]
    Task<RequestSnapshot> Body_Contains_Bytes([FromBody] byte[] body);

    [HttpPost("api/test/bind/body/contains-dto-with-nested-stream")]
    Task<RequestSnapshot> Body_Contains_DtoWithNestedStream([FromBody] DtoWithNestedStream dto);

    [HttpPost("api/test/bind/body/contains-dto-with-nested-bytes")]
    Task<RequestSnapshot> Body_Contains_DtoWithNestedBytes([FromBody] DtoWithNestedBytes dto);

    [HttpPost("api/test/bind/body/contains-dto-with-nested-formdata")]
    Task<RequestSnapshot> Body_Contains_DtoWithNestedFormData([FromBody] DtoWithNestedFormData dto);

    // [HttpPost("api/test/bind/invalid/two-bodies")]
    // Task<RequestSnapshot> Two_Bodies([FromBody] ComplexDto a, [FromBody] ComplexDto b);

    #endregion

    #region Form

    [HttpPost("api/test/bind/form/contains-primitives")]
    Task<RequestSnapshot> Form_Contains_Primitives([FromForm] string name, [FromForm] int id);

    [HttpPost("api/test/bind/form/contains-primitive-with-name")]
    Task<RequestSnapshot> Form_Contains_Primitive_WithName([FromForm(Name = "comment")] string text);

    [HttpPost("api/test/bind/form/contains-primitive-dictionary")]
    Task<RequestSnapshot> Form_Contains_Primitive_Dictionary([FromForm] Dictionary<string, string> values);

    [HttpPost("api/test/bind/form/contains-primitive-collection")]
    Task<RequestSnapshot> Form_Contains_Primitive_Collection([FromForm] List<string> values);

    [HttpPost("api/test/bind/form/contains-iformfile")]
    Task<RequestSnapshot> Form_Contains_IFormFile( /*[FromForm]*/ IFormFile file);

    [HttpPost("api/test/bind/form/contains-iformfile-and-primitives")]
    Task<RequestSnapshot> Form_Contains_IFormFile_And_Primitives( /*[FromForm]*/ IFormFile file,
        [FromForm] string comment);
    
    [HttpPost("api/test/bind/form/contains-iformfile-and-primitive-dictionary")]
    Task<RequestSnapshot> Form_Contains_IFormFile_And_Primitive_Dictionary(
        /*[FromForm]*/ IFormFile file,
        [FromForm] Dictionary<string, string> values);

    [HttpPost("api/test/bind/form/contains-iformfile-collection")]
    Task<RequestSnapshot> Form_Contains_IFormFile_Collection([FromForm] List<IFormFile> files);

    [HttpPost("api/test/bind/form/contains-iformfilecollection")]
    Task<RequestSnapshot> Form_Contains_IFormFileCollection([FromForm] IFormFileCollection files);

    [HttpPost("api/test/bind/form/contains-iformfilecollection-and-primitives")]
    Task<RequestSnapshot> Form_Contains_IFormFileCollection_And_Primitives([FromForm] IFormFileCollection files,
        [FromForm] string comment);

    [HttpPost("api/test/bind/form/contains-dto-with-nested-formdata")]
    Task<RequestSnapshot> Form_Contains_DtoWithNestedFormData([FromForm] DtoWithNestedFormData dto);

    #endregion

    #region Body and Form combining

    [HttpPost("api/test/bind/invalid/body-complexdto-and-form-primitive")]
    Task<RequestSnapshot> BodyComplexDto_And_FormPrimitive([FromBody] ComplexDto dto, [FromForm] string formValue);

    [HttpPost("api/test/bind/invalid/body-complexdto-and-iformfile")]
    Task<RequestSnapshot> BodyComplexDto_And_IFormFile([FromBody] ComplexDto dto, /*[FromForm]*/ IFormFile file);

    #endregion

    #region No Attributes

    // [HttpPost("api/test/bind/noattr/two-complexdtos/order-1-2")]
    // Task<RequestSnapshot> NoAttr_TwoComplexDtos_Order_1_2(ComplexDto dto1, ComplexDto dto2);
    //
    // [HttpPost("api/test/bind/noattr/two-complexdtos/order-2-1")]
    // Task<RequestSnapshot> NoAttr_TwoComplexDtos_Order_2_1(ComplexDto dto2, ComplexDto dto1);
    //
    // [HttpPost("api/test/bind/noattr/complexdto-and-bytes")]
    // Task<RequestSnapshot> NoAttr_ComplexDto_And_Bytes(ComplexDto dto, byte[] bytes);
    //
    // [HttpPost("api/test/bind/noattr/bytes-and-complexdto")]
    // Task<RequestSnapshot> NoAttr_Bytes_And_ComplexDto(byte[] bytes, ComplexDto dto);
    //
    // [HttpPost("api/test/bind/noattr/complexdto-and-stream")]
    // Task<RequestSnapshot> NoAttr_ComplexDto_And_Stream(ComplexDto dto, Stream stream);
    //
    // [HttpPost("api/test/bind/noattr/stream-and-complexdto")]
    // Task<RequestSnapshot> NoAttr_Stream_And_ComplexDto(Stream stream, ComplexDto dto);

    [HttpPost("api/test/bind/noattr/primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
    Task<RequestSnapshot>
        NoAttr_PrimitiveAlsoInRouteTemplate_Primitive_And_ComplexDto(int id, string q, ComplexDto dto);

    [HttpPost("api/test/bind/noattr/complexdto-and-primitive-also-in-route-template-and-primitive/{id:int}")]
    Task<RequestSnapshot> NoAttr_ComplexDto_And_PrimitiveAlsoInRouteTemplate_And_Primitive(ComplexDto dto,
        int id,
        string q);

    [HttpPost("api/test/bind/noattr/iformfile-and-primitive")]
    Task<RequestSnapshot> NoAttr_IFormFile_And_Primitive(IFormFile file, string comment);

    #endregion

    #region Mixed different attributes

    [HttpPost(
        "api/test/bind/allattrs/route-primitive-also-in-route-template-and-query-primitive-and-body-complexdto/{id:int}")]
    Task<RequestSnapshot> AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_BodyComplexDto(
        [FromRoute] int id,
        [FromQuery] string q,
        [FromBody] ComplexDto dto);

    [HttpPost(
        "api/test/bind/allattrs/route-primitive-also-in-route-template-and-header-primitive-and-body-complexdto/{id:int}")]
    Task<RequestSnapshot> AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_HeaderPrimitive_And_BodyComplexDto(
        [FromRoute] int id,
        [FromHeader(Name = "X-Trace")] string traceId,
        [FromBody] ComplexDto dto);

    [HttpGet(
        "api/test/bind/allattrs/route-primitive-also-in-route-template-and-query-primitive-and-header-primitive/{id:int}")]
    Task<RequestSnapshot> AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_HeaderPrimitive(
        [FromRoute] int id,
        [FromQuery] string q,
        [FromHeader(Name = "X-Test")] string header);

    [HttpPost("api/test/bind/allattrs/form-primitive-and-header-primitive")]
    Task<RequestSnapshot> AllAttrs_FormPrimitive_And_HeaderPrimitive(
        [FromForm] string comment,
        [FromHeader(Name = "X-Trace")] string traceId);

    [HttpPost(
        "api/test/bind/allattrs/route-primitive-and-query-primitive-and-header-primitive-and-form-primitive-with-names/{id:int}")]
    Task<RequestSnapshot> AllAttrs_RoutePrimitive_And_QueryPrimitive_And_HeaderPrimitive_And_FormPrimitive_WithNames(
        [FromRoute(Name = "id")] int routeId,
        [FromQuery(Name = "q")] string query,
        [FromHeader(Name = "X-Trace")] string traceId,
        [FromForm(Name = "comment")] string comment);

    #endregion

    #region Mixed different attributes and no attributes

    [HttpPost("api/test/bind/mixed/route-primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
    Task<RequestSnapshot> Mixed_RoutePrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto([FromRoute] int id,
        string q,
        ComplexDto dto);

    [HttpPost("api/test/bind/mixed/primitive-also-in-route-template-and-query-primitive-and-complexdto/{id:int}")]
    Task<RequestSnapshot> Mixed_PrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_ComplexDto(int id,
        [FromQuery] string q,
        ComplexDto dto);

    [HttpPost("api/test/bind/mixed/query-complexdto-and-complexdto")]
    Task<RequestSnapshot> Mixed_QueryComplexDto_And_ComplexDto([FromQuery] ComplexDto dtoQuery, ComplexDto dtoBody);

    [HttpPost("api/test/bind/mixed/body-complexdto-and-primitives")]
    Task<RequestSnapshot> Mixed_BodyComplexDto_And_Primitives([FromBody] ComplexDto dto, int page, string tag);

    // [HttpPost("api/test/bind/mixed/body-complexdto-and-complexdto")]
    // Task<RequestSnapshot> Mixed_BodyComplexDto_And_ComplexDto([FromBody] ComplexDto dto1, ComplexDto dto2);

    [HttpPost("api/test/bind/mixed/iformfile-and-form-primitive")]
    Task<RequestSnapshot> Mixed_IFormFile_And_FormPrimitive(IFormFile file, [FromForm] string comment);

    [HttpPost("api/test/bind/mixed/query-primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
    Task<RequestSnapshot> Mixed_QueryPrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto([FromQuery] int id,
        string q,
        ComplexDto dto);

    #endregion

    #endregion

    #region Response Result Building

    #region Domain return types

    [HttpGet("api/test/return/domain/void")]
    void Domain_Void();

    [HttpGet("api/test/return/domain/int")]
    int Domain_Int();

    [HttpGet("api/test/return/domain/string")]
    string Domain_String();

    [HttpGet("api/test/return/domain/complexdto")]
    ComplexDto Domain_ComplexDto();

    [HttpGet("api/test/return/domain/bytes")]
    byte[] Domain_Bytes();

    [HttpGet("api/test/return/domain/stream")]
    Stream Domain_Stream();

    [HttpGet("api/test/return/domain/task")]
    Task Domain_Task();

    [HttpGet("api/test/return/domain/task-int")]
    Task<int> Domain_Task_Int();

    [HttpGet("api/test/return/domain/task-string")]
    Task<string> Domain_Task_String();

    [HttpGet("api/test/return/domain/task-complexdto")]
    Task<ComplexDto> Domain_Task_ComplexDto();

    [HttpGet("api/test/return/domain/task-bytes")]
    Task<byte[]> Domain_Task_Bytes();

    [HttpGet("api/test/return/domain/task-stream")]
    Task<Stream> Domain_Task_Stream();

    #endregion

    #region ActionResult<T> return types

    [HttpGet("api/test/return/actionresultt/int")]
    ActionResult<int> ActionResultT_Int();

    [HttpGet("api/test/return/actionresultt/complexdto")]
    ActionResult<ComplexDto> ActionResultT_ComplexDto();

    [HttpGet("api/test/return/actionresultt/string")]
    ActionResult<string> ActionResultT_String();

    [HttpGet("api/test/return/actionresultt/bytes")]
    ActionResult<byte[]> ActionResultT_Bytes();

    [HttpGet("api/test/return/actionresultt/stream")]
    ActionResult<Stream> ActionResultT_Stream();

    #endregion

    #region Concrete ActionResult implementations return types

    [HttpGet("api/test/return/concrete-actionresult/contentresult-string")]
    ContentResult ContentResult_String();

    [HttpGet("api/test/return/concrete-actionresult/objectresult-complexdto")]
    ObjectResult ObjectResult_ComplexDto();

    [HttpGet("api/test/return/concrete-actionresult/okobjectresult-complexdto")]
    OkObjectResult OkObjectResult_ComplexDto();

    [HttpGet("api/test/return/concrete-actionresult/jsonresult-complexdto")]
    JsonResult JsonResult_ComplexDto();

    [HttpGet("api/test/return/concrete-actionresult/statuscoderesult-418")]
    StatusCodeResult StatusCodeResult_418();

    [HttpGet("api/test/return/concrete-actionresult/okresult-200")]
    OkResult OkResult_200();

    [HttpGet("api/test/return/concrete-actionresult/filecontentresult-bytes")]
    FileContentResult FileContentResult_Bytes();

    [HttpGet("api/test/return/concrete-actionresult/filestreamresult-stream")]
    FileStreamResult FileStreamResult_Stream();

    #endregion

    #region Abstract and not explicitly supported ActionResults return types

    [HttpGet("api/test/return/abstract-and-default-actionresult/iactionresult-objectresult-complexdto")]
    IActionResult IActionResult_ObjectResult_ComplexDto();

    [HttpGet("api/test/return/abstract-and-default-actionresult/actionresult-contentresult-string")]
    ActionResult ActionResult_ContentResult_String();

    [HttpGet("api/test/return/abstract-and-default-actionresult/iactionresult-filecontentresult-bytes")]
    IActionResult IActionResult_FileContentResult_Bytes();

    [HttpGet("api/test/return/abstract-and-default-actionresult/actionresult-filestreamresult-stream")]
    ActionResult ActionResult_FileStreamResult_Stream();

    [HttpGet("api/test/return/abstract-and-default-actionresult/iactionresult-statuscoderesult-418")]
    IActionResult IActionResult_StatusCodeResult_418();

    [HttpGet("api/test/return/abstract-and-default-actionresult/redirectresult")]
    RedirectResult RedirectResult();

    [HttpGet("api/test/return/abstract-and-default-actionresult/forbidresult")]
    ForbidResult ForbidResult();

    [HttpGet("api/test/return/abstract-and-default-actionresult/challengeresult")]
    ChallengeResult ChallengeResult();

    #endregion

    #endregion
}