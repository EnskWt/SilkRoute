using System.IO;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Demo.Shared.Contracts;
using SilkRoute.Demo.Shared.Models;

namespace SilkRoute.Demo.ClientMicroservice.Controllers;

[ApiController]
public sealed class ClientController : ControllerBase
{
    private readonly ITestMicroservice _testMicroservice;

    public ClientController(ITestMicroservice test)
        => _testMicroservice = test;

    #region Request Parameter Binding

    #region Query

    [HttpGet("api/test/bind/query/test-contains-primitives")]
    public async Task<IActionResult> Test_Query_Contains_Primitives([FromQuery] int id, [FromQuery] bool flag, [FromQuery] string name)
        => Ok(await _testMicroservice.Query_Contains_Primitives(id, flag, name));

    [HttpGet("api/test/bind/query/test-contains-primitive-collections")]
    public async Task<IActionResult> Test_Query_Contains_Primitive_Collections([FromQuery] int[] ids, [FromQuery] List<string> tags)
        => Ok(await _testMicroservice.Query_Contains_Primitive_Collections(ids, tags));

    [HttpGet("api/test/bind/query/test-contains-complexdto")]
    public async Task<IActionResult> Test_Query_Contains_ComplexDto([FromQuery] ComplexDto dto)
        => Ok(await _testMicroservice.Query_Contains_ComplexDto(dto));

    [HttpGet("api/test/bind/query/test-contains-nesteddto")]
    public async Task<IActionResult> Test_Query_Contains_NestedDto([FromQuery] NestedDto dto)
        => Ok(await _testMicroservice.Query_Contains_NestedDto(dto));

    [HttpGet("api/test/bind/query/test-contains-complexdto-and-primitive")]
    public async Task<IActionResult> Test_Query_Contains_ComplexDto_And_Primitive([FromQuery] ComplexDto dto, [FromQuery] int id)
        => Ok(await _testMicroservice.Query_Contains_ComplexDto_And_Primitive(dto, id));

    [HttpGet("api/test/bind/query/test-contains-nullables")]
    public async Task<IActionResult> Test_Query_Contains_Nullables([FromQuery] int? id, [FromQuery] string? name)
        => Ok(await _testMicroservice.Query_Contains_Nullables(id, name));

    [HttpGet("api/test/bind/query/test-contains-stream")] // TO TEST
    public async Task<IActionResult> Test_Query_Contains_Stream([FromQuery] DtoWithNestedStream dto)
        => Ok(await _testMicroservice.Query_Contains_Stream(dto));

    [HttpGet("api/test/bind/query/test-contains-bytes")] // TO TEST
    public async Task<IActionResult> Test_Query_Contains_Bytes([FromQuery] DtoWithNestedBytes dto)
        => Ok(await _testMicroservice.Query_Contains_Bytes(dto));

    #endregion

    #region Route and Header

    [HttpGet("api/test/bind/route/test-contains-primitive/{id:int}")]
    public async Task<IActionResult> Test_Route_Contains_Primitive([FromRoute] int id)
        => Ok(await _testMicroservice.Route_Contains_Primitive(id));

    [HttpGet("api/test/bind/header/test-contains-primitive")]
    public async Task<IActionResult> Test_Header_Contains_Primitive([FromHeader(Name = "X-Test")] string value)
        => Ok(await _testMicroservice.Header_Contains_Primitive(value));

    #endregion

    #region Body

    [HttpPost("api/test/bind/body/test-contains-complexdto")]
    public async Task<IActionResult> Test_Body_Contains_ComplexDto([FromBody] ComplexDto dto)
        => Ok(await _testMicroservice.Body_Contains_ComplexDto(dto));

    [HttpPost("api/test/bind/body/test-contains-primitive")]
    public async Task<IActionResult> Test_Body_Contains_Primitive([FromBody] string text)
        => Ok(await _testMicroservice.Body_Contains_Primitive(text));

    [HttpPost("api/test/bind/body/test-contains-stream")]
    public async Task<IActionResult> Test_Body_Contains_Stream([FromBody] Stream body)
        => Ok(await _testMicroservice.Body_Contains_Stream(body));

    [HttpPost("api/test/bind/body/test-contains-bytes")]
    public async Task<IActionResult> Test_Body_Contains_Bytes([FromBody] byte[] body)
        => Ok(await _testMicroservice.Body_Contains_Bytes(body));

    [HttpPost("api/test/bind/body/test-contains-dto-with-nested-stream")] // TO TEST
    public async Task<IActionResult> Test_Body_Contains_DtoWithNestedStream([FromBody] DtoWithNestedStream dto)
        => Ok(await _testMicroservice.Body_Contains_DtoWithNestedStream(dto));

    [HttpPost("api/test/bind/body/test-contains-dto-with-nested-bytes")]
    public async Task<IActionResult> Test_Body_Contains_DtoWithNestedBytes([FromBody] DtoWithNestedBytes dto)
        => Ok(await _testMicroservice.Body_Contains_DtoWithNestedBytes(dto));

    [HttpPost("api/test/bind/body/test-contains-dto-with-nested-formdata")] // TO TEST
    public async Task<IActionResult> Test_Body_Contains_DtoWithNestedFormData([FromBody] DtoWithNestedFormData dto)
        => Ok(await _testMicroservice.Body_Contains_DtoWithNestedFormData(dto));

    //[HttpPost("api/test/bind/invalid/test-two-bodies")]
    //public async Task<IActionResult> Test_Two_Bodies([FromBody] ComplexDto a, [FromBody] ComplexDto b)
    //    => Ok(await _testMicroservice.Two_Bodies(a, b));

    #endregion

    #region Form

    [HttpPost("api/test/bind/form/test-contains-primitives")]
    public async Task<IActionResult> Test_Form_Contains_Primitives([FromForm] string name, [FromForm] int id)
        => Ok(await _testMicroservice.Form_Contains_Primitives(name, id));

    [HttpPost("api/test/bind/form/test-contains-iformfile")]
    public async Task<IActionResult> Test_Form_Contains_IFormFile(/*[FromForm]*/ IFormFile file)
        => Ok(await _testMicroservice.Form_Contains_IFormFile(file));

    [HttpPost("api/test/bind/form/test-contains-iformfile-and-primitives")]
    public async Task<IActionResult> Test_Form_Contains_IFormFile_And_Primitives(/*[FromForm]*/ IFormFile file, [FromForm] string comment)
        => Ok(await _testMicroservice.Form_Contains_IFormFile_And_Primitives(file, comment));

    [HttpPost("api/test/bind/form/test-contains-iformfiles")]
    public async Task<IActionResult> Test_Form_Contains_IFormFiles([FromForm] List<IFormFile> files)
        => Ok(await _testMicroservice.Form_Contains_IFormFiles(files));

    #endregion

    #region Body and Form combining

    [HttpPost("api/test/bind/invalid/test-body-complexdto-and-form-primitive")]
    public async Task<IActionResult> Test_BodyComplexDto_And_FormPrimitive([FromBody] ComplexDto dto, [FromForm] string formValue) // TO TEST
        => Ok(await _testMicroservice.BodyComplexDto_And_FormPrimitive(dto, formValue));

    [HttpPost("api/test/bind/invalid/test-body-complexdto-and-iformfile")]
    public async Task<IActionResult> Test_BodyComplexDto_And_IFormFile([FromBody] ComplexDto dto, IFormFile file) // TO TEST
        => Ok(await _testMicroservice.BodyComplexDto_And_IFormFile(dto, file));

    #endregion

    #region No Attributes

    //[HttpPost("api/test/bind/noattr/test-two-complexdtos/order-1-2")]
    //public async Task<IActionResult> Test_NoAttr_TwoComplexDtos_Order_1_2(ComplexDto dto1, ComplexDto dto2)
    //    => Ok(await _testMicroservice.NoAttr_TwoComplexDtos_Order_1_2(dto1, dto2));

    //[HttpPost("api/test/bind/noattr/test-two-complexdtos/order-2-1")]
    //public async Task<IActionResult> Test_NoAttr_TwoComplexDtos_Order_2_1(ComplexDto dto2, ComplexDto dto1)
    //    => Ok(await _testMicroservice.NoAttr_TwoComplexDtos_Order_2_1(dto2, dto1));

    //[HttpPost("api/test/bind/noattr/test-complexdto-and-bytes")]
    //public async Task<IActionResult> Test_NoAttr_ComplexDto_And_Bytes(ComplexDto dto, byte[] bytes)
    //    => Ok(await _testMicroservice.NoAttr_ComplexDto_And_Bytes(dto, bytes));

    //[HttpPost("api/test/bind/noattr/test-bytes-and-complexdto")]
    //public async Task<IActionResult> Test_NoAttr_Bytes_And_ComplexDto(byte[] bytes, ComplexDto dto)
    //    => Ok(await _testMicroservice.NoAttr_Bytes_And_ComplexDto(bytes, dto));

    //[HttpPost("api/test/bind/noattr/test-complexdto-and-stream")]
    //public async Task<IActionResult> Test_NoAttr_ComplexDto_And_Stream(ComplexDto dto, Stream stream)
    //    => Ok(await _testMicroservice.NoAttr_ComplexDto_And_Stream(dto, stream));

    //[HttpPost("api/test/bind/noattr/test-stream-and-complexdto")]
    //public async Task<IActionResult> Test_NoAttr_Stream_And_ComplexDto(Stream stream, ComplexDto dto)
    //    => Ok(await _testMicroservice.NoAttr_Stream_And_ComplexDto(stream, dto));

    [HttpPost("api/test/bind/noattr/test-primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
    public async Task<IActionResult> Test_NoAttr_PrimitiveAlsoInRouteTemplate_Primitive_And_ComplexDto(int id, string q, ComplexDto dto)
        => Ok(await _testMicroservice.NoAttr_PrimitiveAlsoInRouteTemplate_Primitive_And_ComplexDto(id, q, dto));

    [HttpPost("api/test/bind/noattr/test-complexdto-and-primitive-also-in-route-template-and-primitive/{id:int}")]
    public async Task<IActionResult> Test_NoAttr_ComplexDto_And_PrimitiveAlsoInRouteTemplate_And_Primitive(ComplexDto dto, int id, string q)
        => Ok(await _testMicroservice.NoAttr_ComplexDto_And_PrimitiveAlsoInRouteTemplate_And_Primitive(dto, id, q));

    [HttpPost("api/test/bind/noattr/test-iformfile-and-primitive")]
    public async Task<IActionResult> Test_NoAttr_IFormFile_And_Primitive(IFormFile file, string comment)
        => Ok(await _testMicroservice.NoAttr_IFormFile_And_Primitive(file, comment));

    #endregion

    #region Mixed different attributes

    [HttpPost("api/test/bind/allattrs/test-route-primitive-also-in-route-template-and-query-primitive-and-body-complexdto/{id:int}")]
    public async Task<IActionResult> Test_AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_BodyComplexDto([FromRoute] int id, [FromQuery] string q, [FromBody] ComplexDto dto)
        => Ok(await _testMicroservice.AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_BodyComplexDto(id, q, dto));

    [HttpPost("api/test/bind/allattrs/test-route-primitive-also-in-route-template-and-header-primitive-and-body-complexdto/{id:int}")]
    public async Task<IActionResult> Test_AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_HeaderPrimitive_And_BodyComplexDto([FromRoute] int id, [FromHeader(Name = "X-Trace")] string traceId, [FromBody] ComplexDto dto)
        => Ok(await _testMicroservice.AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_HeaderPrimitive_And_BodyComplexDto(id, traceId, dto));

    [HttpGet("api/test/bind/allattrs/test-route-primitive-also-in-route-template-and-query-primitive-and-header-primitive/{id:int}")]
    public async Task<IActionResult> Test_AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_HeaderPrimitive([FromRoute] int id, [FromQuery] string q, [FromHeader(Name = "X-Test")] string header)
        => Ok(await _testMicroservice.AllAttrs_RoutePrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_HeaderPrimitive(id, q, header));

    [HttpPost("api/test/bind/allattrs/test-form-primitive-and-header-primitive")]
    public async Task<IActionResult> Test_AllAttrs_FormPrimitive_And_HeaderPrimitive([FromForm] string comment, [FromHeader(Name = "X-Trace")] string traceId)
        => Ok(await _testMicroservice.AllAttrs_FormPrimitive_And_HeaderPrimitive(comment, traceId));

    #endregion

    #region Mixed different attributes and no attributes

    [HttpPost("api/test/bind/mixed/test-route-primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
    public async Task<IActionResult> Test_Mixed_RoutePrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto([FromRoute] int id, string q, ComplexDto dto)
        => Ok(await _testMicroservice.Mixed_RoutePrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto(id, q, dto));

    [HttpPost("api/test/bind/mixed/test-primitive-also-in-route-template-and-query-primitive-and-complexdto/{id:int}")]
    public async Task<IActionResult> Test_Mixed_PrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_ComplexDto(int id, [FromQuery] string q, ComplexDto dto)
        => Ok(await _testMicroservice.Mixed_PrimitiveAlsoInRouteTemplate_And_QueryPrimitive_And_ComplexDto(id, q, dto));

    [HttpPost("api/test/bind/mixed/test-query-complexdto-and-complexdto")]
    public async Task<IActionResult> Test_Mixed_QueryComplexDto_And_ComplexDto([FromQuery] ComplexDto dtoQuery, ComplexDto dtoBody)
        => Ok(await _testMicroservice.Mixed_QueryComplexDto_And_ComplexDto(dtoQuery, dtoBody));

    [HttpPost("api/test/bind/mixed/test-body-complexdto-and-primitives")]
    public async Task<IActionResult> Test_Mixed_BodyComplexDto_And_Primitives([FromBody] ComplexDto dto, int page, string tag)
        => Ok(await _testMicroservice.Mixed_BodyComplexDto_And_Primitives(dto, page, tag));

    //[HttpPost("api/test/bind/mixed/test-body-complexdto-and-complexdto")]
    //public async Task<IActionResult> Test_Mixed_BodyComplexDto_And_ComplexDto([FromBody] ComplexDto dto1, ComplexDto dto2)
    //    => Ok(await _testMicroservice.Mixed_BodyComplexDto_And_ComplexDto(dto1, dto2));

    [HttpPost("api/test/bind/mixed/test-iformfile-and-form-primitive")]
    public async Task<IActionResult> Test_Mixed_IFormFile_And_FormPrimitive(IFormFile file, [FromForm] string comment)
        => Ok(await _testMicroservice.Mixed_IFormFile_And_FormPrimitive(file, comment));

    [HttpPost("api/test/bind/mixed/test-query-primitive-also-in-route-template-and-primitive-and-complexdto/{id:int}")]
    public async Task<IActionResult> Test_Mixed_QueryPrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto([FromQuery] int id, string q, ComplexDto dto)
        => Ok(await _testMicroservice.Mixed_QueryPrimitiveAlsoInRouteTemplate_And_Primitive_And_ComplexDto(id, q, dto));

    #endregion

    #endregion

    #region Response Result Building

    #region Domain return types

    [HttpGet("api/test/return/domain/test-void")]
    public IActionResult Test_Domain_Void()
    {
        _testMicroservice.Domain_Void();
        return NoContent();
    }

    [HttpGet("api/test/return/domain/test-int")]
    public ActionResult<int> Test_Domain_Int()
        => _testMicroservice.Domain_Int();

    [HttpGet("api/test/return/domain/test-string")]
    public ActionResult<string> Test_Domain_String()
        => _testMicroservice.Domain_String();

    [HttpGet("api/test/return/domain/test-complexdto")]
    public ActionResult<ComplexDto> Test_Domain_ComplexDto()
        => _testMicroservice.Domain_ComplexDto();

    [HttpGet("api/test/return/domain/test-bytes")]
    public IActionResult Test_Domain_Bytes()
    {
        var bytes = _testMicroservice.Domain_Bytes();

        return new FileContentResult(bytes, "application/pdf") { FileDownloadName = "test.pdf" };
    }

    [HttpGet("api/test/return/domain/test-stream")]
    public IActionResult Test_Domain_Stream()
    {
        var stream = _testMicroservice.Domain_Stream();
        return new FileStreamResult(stream, "application/pdf") { FileDownloadName = "test.pdf" };
    }

    [HttpGet("api/test/return/domain/test-task")]
    public async Task<IActionResult> Test_Domain_Task()
    {
        await _testMicroservice.Domain_Task();
        return NoContent();
    }

    [HttpGet("api/test/return/domain/test-task-int")]
    public async Task<ActionResult<int>> Test_Domain_Task_Int()
        => await _testMicroservice.Domain_Task_Int();

    [HttpGet("api/test/return/domain/test-task-string")]
    public async Task<ActionResult<string>> Test_Domain_Task_String()
        => await _testMicroservice.Domain_Task_String();

    [HttpGet("api/test/return/domain/test-task-complexdto")]
    public async Task<ActionResult<ComplexDto>> Test_Domain_Task_ComplexDto()
        => await _testMicroservice.Domain_Task_ComplexDto();

    [HttpGet("api/test/return/domain/test-task-bytes")]
    public async Task<IActionResult> Test_Domain_Task_Bytes()
    {
        var bytes = await _testMicroservice.Domain_Task_Bytes();
        return new FileContentResult(bytes, "application/pdf") { FileDownloadName = "test.pdf" };
    }

    [HttpGet("api/test/return/domain/test-task-stream")]
    public async Task<IActionResult> Test_Domain_Task_Stream()
    {
        var stream = await _testMicroservice.Domain_Task_Stream();
        return new FileStreamResult(stream, "application/pdf") { FileDownloadName = "test.pdf" };
    }

    #endregion

    #region ActionResult<T> return types

    [HttpGet("api/test/return/actionresultt/test-int")]
    public ActionResult<int> Test_ActionResultT_Int()
        => _testMicroservice.ActionResultT_Int();

    [HttpGet("api/test/return/actionresultt/test-complexdto")]
    public ActionResult<ComplexDto> Test_ActionResultT_ComplexDto()
        => _testMicroservice.ActionResultT_ComplexDto();

    [HttpGet("api/test/return/actionresultt/test-string")]
    public ActionResult<string> Test_ActionResultT_String()
        => _testMicroservice.ActionResultT_String();

    [HttpGet("api/test/return/actionresultt/test-bytes")]
    public IActionResult Test_ActionResultT_Bytes()
    {
        var result = _testMicroservice.ActionResultT_Bytes();
        var bytes = result.Value!;

        return new FileContentResult(bytes, "application/pdf") { FileDownloadName = "test.pdf" };
    }

    [HttpGet("api/test/return/actionresultt/test-stream")]
    public ActionResult<Stream> Test_ActionResultT_Stream()
    {
        var result = _testMicroservice.ActionResultT_Stream();
        var stream = result.Value!;

        return new FileStreamResult(stream, "application/pdf") { FileDownloadName = "test.pdf" };
    }

    #endregion

    #region Concrete ActionResult implementations return types

    [HttpGet("api/test/return/concrete-actionresult/test-contentresult-string")]
    public ContentResult Test_ContentResult_String()
        => _testMicroservice.ContentResult_String();

    [HttpGet("api/test/return/concrete-actionresult/test-objectresult-complexdto")]
    [Produces("application/json")]
    public ObjectResult Test_ObjectResult_ComplexDto()
        => _testMicroservice.ObjectResult_ComplexDto();

    [HttpGet("api/test/return/concrete-actionresult/test-statuscoderesult-418")]
    public StatusCodeResult Test_StatusCodeResult_418()
        => _testMicroservice.StatusCodeResult_418();

    [HttpGet("api/test/return/concrete-actionresult/test-filecontentresult-bytes")]
    public FileContentResult Test_FileContentResult_Bytes()
        => _testMicroservice.FileContentResult_Bytes();

    [HttpGet("api/test/return/concrete-actionresult/test-filestreamresult-stream")]
    public FileStreamResult Test_FileStreamResult_Stream()
        => _testMicroservice.FileStreamResult_Stream();

    #endregion

    #region Abstract and not explicitly supported ActionResults return types

    [HttpGet("api/test/return/abstract-and-default-actionresult/test-iactionresult-objectresult-complexdto")]
    [Produces("application/json")]
    public IActionResult Test_IActionResult_ObjectResult_ComplexDto()
        => _testMicroservice.IActionResult_ObjectResult_ComplexDto();

    [HttpGet("api/test/return/abstract-and-default-actionresult/test-actionresult-contentresult-string")]
    public ActionResult Test_ActionResult_ContentResult_String()
        => _testMicroservice.ActionResult_ContentResult_String();

    [HttpGet("api/test/return/abstract-and-default-actionresult/test-iactionresult-filecontentresult-bytes")]
    public IActionResult Test_IActionResult_FileContentResult_Bytes()
        => _testMicroservice.IActionResult_FileContentResult_Bytes();

    [HttpGet("api/test/return/abstract-and-default-actionresult/test-actionresult-filestreamresult-stream")]
    public ActionResult Test_ActionResult_FileStreamResult_Stream()
        => _testMicroservice.ActionResult_FileStreamResult_Stream();

    [HttpGet("api/test/return/abstract-and-default-actionresult/test-iactionresult-statuscoderesult-418")]
    public IActionResult Test_IActionResult_StatusCodeResult_418()
        => _testMicroservice.IActionResult_StatusCodeResult_418();

    [HttpGet("api/test/return/abstract-and-default-actionresult/test-redirectresult")]
    public RedirectResult Test_RedirectResult()
        => _testMicroservice.RedirectResult();

    [HttpGet("api/test/return/abstract-and-default-actionresult/test-forbidresult")]
    public ForbidResult Test_ForbidResult()
        => _testMicroservice.ForbidResult();

    [HttpGet("api/test/return/abstract-and-default-actionresult/test-challengeresult")]
    public ChallengeResult Test_ChallengeResult()
        => _testMicroservice.ChallengeResult();

    #endregion

    #endregion
}