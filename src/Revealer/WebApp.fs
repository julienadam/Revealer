module WebApp

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Giraffe
open System.IO
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.StaticFiles

let startAsync inputFolder port loglevel =
    let mimeProvider = new FileExtensionContentTypeProvider()
    
    let getMimeTypeForFileExtension filePath = 
        match mimeProvider.TryGetContentType(filePath) with
        | true, c -> c
        | _ -> "application/octet-stream"

    let tryResourceHandler path : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                use stream = Resources.getResourceStream path
                if stream <> null then
                    ctx.SetContentType(getMimeTypeForFileExtension(path))

                    return! ctx.WriteStreamAsync(
                        true,
                        stream,
                        None,
                        None)
                else
                    return! (next ctx)
            }

    let mdToHtmlHandler filename : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let forcedTheme = ctx.TryGetQueryStringValue("theme")
                let forcedHighlightTheme = ctx.TryGetQueryStringValue("highlight-theme")
                let mdFile = Path.Combine(inputFolder, sprintf "%s.md" filename)
                let contents = File.ReadAllText(mdFile)
                if File.Exists(mdFile) then
                    return! (RevealPageBuilder.parseAndRender contents forcedTheme forcedHighlightTheme |> htmlView) next ctx
                else
                    return! (RequestErrors.NOT_FOUND (sprintf "No markdown file named %s.md found" filename)) next ctx
            }

    let router =
        choose [
            routef "/%s.html" mdToHtmlHandler
            routexp "/.*" (fun groups -> tryResourceHandler (groups |> Seq.head))
        ]

    let configureApp (app : IApplicationBuilder) =
        app.UseGiraffe router
        app.UseStaticFiles() |> ignore

    let configureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore

    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseWebRoot(inputFolder) // -> input folder is set as web root
                    .Configure(configureApp)
                    .ConfigureLogging(fun builder ->
                        builder.SetMinimumLevel loglevel |> ignore // --> Set logLevel
                    )
                    .ConfigureServices(configureServices)
                    .UseUrls(sprintf "http://0.0.0.0:%i" port) // -> Server listens to localhost on specified port
                    |> ignore)
        .Build()
        .RunAsync()