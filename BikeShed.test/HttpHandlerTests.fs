namespace BikeShed

module HttpHandlerTests =

    open Microsoft.Extensions.Primitives
    open Giraffe
    open System.IO
    open System.Text
    open System.Threading.Tasks
    open Microsoft.AspNetCore.Http
    open NSubstitute
    open Xunit
    open HttpHandlers

    let private next : HttpFunc = Some >> Task.FromResult

    let private getBody (ctx : HttpContext) =
        ctx.Response.Body.Position <- 0L
        use reader = new StreamReader(ctx.Response.Body, Encoding.UTF8)
        reader.ReadToEnd()

    [<Fact>]
    let ``getBikeHandler works for giant`` () =
        let ctx = Substitute.For<HttpContext>()
        let app = GET >=> choose [ routef "/api/bikes/%s" getBikeHandler ]
        ctx.Request.Method.ReturnsForAnyArgs "GET" |> ignore
        ctx.Request.Path.ReturnsForAnyArgs (PathString("/api/bikes/giant")) |> ignore
        ctx.Response.Body <- new MemoryStream()

        task {
            let! result = app next ctx
            match result with
            | None     -> Assert.True(false)
            | Some ctx ->
                let body = getBody ctx
                Assert.True(body.Contains("\"name\":\"giant\""))
                Assert.True(body.Contains("\"color\":\"blue\""))
        }

    [<Fact>]
    let ``postBikeHandler works`` () =
        let ctx = Substitute.For<HttpContext>()
        let app = POST >=> choose [ route "/api/bikes" >=> postBikeHandler ]

        let headers = HeaderDictionary()
        headers.Add("Content-Type", StringValues("application/json"))
        headers.Add("Accept", StringValues("application/json"))
        ctx.Request.Method.ReturnsForAnyArgs "POST" |> ignore
        ctx.Request.Path.ReturnsForAnyArgs (PathString("/api/bikes")) |> ignore
        ctx.Request.Headers.ReturnsForAnyArgs(headers) |> ignore
        ctx.Request.HasFormContentType.ReturnsForAnyArgs(false) |> ignore
        ctx.Request.ContentType.ReturnsForAnyArgs("application/json") |> ignore
        ctx.Request.Body <- new MemoryStream()
        ctx.Response.Body <- new MemoryStream()
        let t = async {
            let s = "{ \"Name\": \"posttestbike\", \"Color\": \"yellow\" }"
            let buffer = System.Text.Encoding.UTF8.GetBytes(s)
            do! ctx.Request.Body.AsyncWrite(buffer, 0, buffer.Length)
        }
        Async.RunSynchronously t
        ctx.Request.Body.Seek(0L, SeekOrigin.Begin) |> ignore

        task {
            let! result = app next ctx
            match result with
            | None     -> Assert.True(false)
            | Some ctx -> Assert.True(ctx.Response.StatusCode = StatusCodes.Status201Created)
       }
