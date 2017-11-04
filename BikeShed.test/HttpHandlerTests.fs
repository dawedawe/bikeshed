namespace BikeShed

module HttpHandlerTests =

    open Giraffe
    open System.IO
    open System.Text
    open System.Threading.Tasks
    open Microsoft.AspNetCore.Http
    open NSubstitute
    open Xunit
    open HttpHandlers

    let next : HttpFunc = Some >> Task.FromResult

    let getBody (ctx : HttpContext) =
        ctx.Response.Body.Position <- 0L
        use reader = new StreamReader(ctx.Response.Body, Encoding.UTF8)
        reader.ReadToEnd()


    [<Fact>]
    let ``getBikeHandler works for giant`` () =
        let ctx = Substitute.For<HttpContext>()
        let app =
            GET >=> choose [
                routef "/api/bikes/%s" getBikeHandler
            ]
        ctx.Request.Method.ReturnsForAnyArgs "GET" |> ignore
        ctx.Request.Path.ReturnsForAnyArgs (PathString("/api/bikes/giant")) |> ignore
        ctx.Response.Body <- new MemoryStream()

        task {
            let! result = app next ctx
            match result with
            | None     -> Assert.True(false)
            | Some ctx ->
                let body = getBody ctx
                Assert.True(body.Contains("\"Name\":\"giant\""))
                Assert.True(body.Contains("\"Color\":\"blue\""))
       }