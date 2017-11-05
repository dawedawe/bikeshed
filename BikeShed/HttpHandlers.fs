namespace BikeShed

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open Giraffe.HttpContextExtensions
    open Giraffe.HttpHandlers
    open Giraffe.Tasks
    open Giraffe.Razor.HttpHandlers
    open Model
    open Logic

    let getBikesHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            if ctx.Response.HasStarted then
                next ctx
            else
                let bikes = getBikes()
                json bikes next ctx

    let postBikeHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            if ctx.Response.HasStarted then
                next ctx
            else
                task {
                    let! bike = ctx.BindModel<Bike>()
                    let result = createBike(bike)
                    match result with
                    | Success _ -> ctx.Response.StatusCode <- StatusCodes.Status201Created
                    | Failure e -> ctx.Response.StatusCode <- StatusCodes.Status400BadRequest
                                   ctx.WriteText e |> ignore
                    return! next ctx
                }

    let getBikeHandler name : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            if ctx.Response.HasStarted then
                next ctx
            else
                task {
                    let result = getBike name
                    match result with
                    | Success bike -> return! json bike next ctx
                    | Failure _ -> return! setStatusCode StatusCodes.Status404NotFound next ctx
                }
    
    let putBikeHandler name : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            if ctx.Response.HasStarted then
                next ctx
            else
                task {
                    let! bike = ctx.BindModel<Bike>()
                    let result = updateBike name bike
                    match result with
                    | Success _ -> ctx.Response.StatusCode <- StatusCodes.Status200OK
                    | Failure e -> ctx.Response.StatusCode <- StatusCodes.Status400BadRequest
                                   ctx.WriteText e |> ignore
                    return! next ctx
                }

    let deleteBikeHandler name : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            if ctx.Response.HasStarted then
                next ctx
            else
                task {
                    let result = deleteBike(name)
                    match result with
                    | Success _ -> ctx.Response.StatusCode <- StatusCodes.Status200OK
                    | Failure e -> ctx.Response.StatusCode <- StatusCodes.Status404NotFound
                                   ctx.WriteText e |> ignore
                    return! next ctx
                }

    let getBikeRazorViewHandler name : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            if ctx.Response.HasStarted then
                next ctx
            else
                task {
                    let result = getBike name
                    match result with
                    | Success bike -> return! razorHtmlView "Bike" bike next ctx
                    | Failure _ -> return! setStatusCode StatusCodes.Status404NotFound next ctx
                }
