using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorApp1;

public static class ReportsExtensions
{

	public static WebApplication MapWeatherReport(this WebApplication app, IWebHostEnvironment host)
	{

		AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
		IronPdf.Installation.TempFolderPath = $@"{host.ContentRootPath}/irontemp/";
		IronPdf.Installation.Initialize();
		IronPdf.Installation.LinuxAndDockerDependenciesAutoConfig = true;
		var Renderer = new ChromePdfRenderer();
		using var PDF = Renderer.RenderHtmlAsPdf("<h2>Hello World</h2>");

		app.MapGet("/reports/Weather", async (HttpContext ctx) =>
		{

			var url = ctx.Request.GetDisplayUrl();
			url = url.Substring(0, url.IndexOf("/reports"));

			var ironPdfRender = new IronPdf.ChromePdfRenderer();
			ironPdfRender.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.Letter;
			ironPdfRender.RenderingOptions.RenderDelay = 2500;
			ironPdfRender.RenderingOptions.ViewPortWidth = 1080;
			ironPdfRender.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Screen;
			ironPdfRender.RenderingOptions.MarginTop = 10;
			ironPdfRender.RenderingOptions.MarginLeft = 10;
			ironPdfRender.RenderingOptions.MarginRight = 10;
			ironPdfRender.RenderingOptions.MarginBottom = 10;
			ironPdfRender.RenderingOptions.TextFooter = new TextHeaderFooter
			{
				RightText = $"As of {DateTime.UtcNow.ToString()}    "
			};
			ironPdfRender.RenderingOptions.UseMarginsOnHeaderAndFooter = UseMargins.Bottom;
			ironPdfRender.RenderingOptions.Title = $"My Weather Forecast";
			ironPdfRender.RenderingOptions.FitToPaperMode = IronPdf.Engines.Chrome.FitToPaperModes.AutomaticFit;
			var pdf = await ironPdfRender.RenderUrlAsPdfAsync($"{url}/weather");

			ctx.Response.ContentType = "application/pdf";
			ctx.Response.Headers.Add("Content-Disposition", $"filename=weatherforecast.pdf");
			await ctx.Response.BodyWriter.WriteAsync(pdf.Stream.ToArray());

		});

		return app;

	}

}
