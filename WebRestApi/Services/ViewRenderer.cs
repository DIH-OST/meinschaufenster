// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebRestApi.Services
{
    /// <summary>
    ///     Renderer für Mails
    /// </summary>
    public class ViewRender
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IRazorViewEngine _viewEngine;

        /// <summary>
        ///     Konstruktor
        /// </summary>
        /// <param name="viewEngine"></param>
        /// <param name="tempDataProvider"></param>
        /// <param name="serviceProvider"></param>
        public ViewRender(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     Renderer
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="name"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string Render<TModel>(string name, TModel model)
        {
            var actionContext = GetActionContext();
            ViewEngineResult viewEngineResult;

            try
            {
                viewEngineResult = _viewEngine.FindView(actionContext, name, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            if (!viewEngineResult.Success)
                throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));

            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(
                        new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        _tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                view.RenderAsync(viewContext).GetAwaiter().GetResult();

                return output.ToString();
            }
        }

        /// <summary>
        ///     Action Kontext abfragen
        /// </summary>
        /// <returns></returns>
        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _serviceProvider;
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}