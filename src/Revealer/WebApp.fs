module WebApp

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System.IO
open Microsoft.AspNetCore.Http

let startAsync inputFolder port =

    let resourceHandler (path:string) : HttpHandler =
        fun (_ : HttpFunc) (ctx : HttpContext) ->
            task {
                use stream = Resources.getResourceStream path
                if stream = null then
                    ctx.SetStatusCode(404)

                return! ctx.WriteStreamAsync(
                    true,
                    stream,
                    None,
                    None)
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
            routexp "/(dist|plugin|revealer|lib)/(.*)" (fun groups -> resourceHandler (groups |> Seq.head))
            routef "/%s.html" mdToHtmlHandler
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
                    .ConfigureServices(configureServices)
                    .UseUrls(sprintf "http://0.0.0.0:%i" port) // -> Server listens to localhost on specified port
                    |> ignore)
        .Build()
        .RunAsync()