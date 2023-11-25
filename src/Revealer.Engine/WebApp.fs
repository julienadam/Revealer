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

    let router =
        choose [
            routexp "/(dist|plugin|revealer)/(.*)" (fun groups -> resourceHandler (groups |> Seq.head))
            routef "/%s.html" (fun filename -> 
                let mdFile = Path.Combine(inputFolder, sprintf "%s.md" filename)
                if File.Exists(mdFile) then
                    MarkdownToReveal.parseAndRender(File.ReadAllText(mdFile)) |> htmlView
                else
                   RequestErrors.NOT_FOUND (sprintf "No markdown file named %s.md found" filename)
            )
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