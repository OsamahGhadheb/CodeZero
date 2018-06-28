//  <copyright file="JQueryProxyScriptGenerator.cs" project="CodeZero.Web.Common" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using System.Text;
using CodeZero.Dependency;
using CodeZero.Extensions;
using CodeZero.Web.Api.Modeling;

namespace CodeZero.Web.Api.ProxyScripting.Generators.JQuery
{
    public class JQueryProxyScriptGenerator : IProxyScriptGenerator, ITransientDependency
    {
        /// <summary>
        /// "jquery".
        /// </summary>
        public const string Name = "jquery";

        public string CreateScript(ApplicationApiDescriptionModel model)
        {
            var script = new StringBuilder();

            script.AppendLine("/* This file is automatically generated by CodeZero framework to use MVC Controllers from JavaScript. */");
            script.AppendLine();
            script.AppendLine("var CodeZero = CodeZero || {};");
            script.AppendLine("CodeZero.services = CodeZero.services || {};");

            foreach (var module in model.Modules.Values)
            {
                script.AppendLine();
                AddModuleScript(script, module);
            }

            return script.ToString();
        }

        private static void AddModuleScript(StringBuilder script, ModuleApiDescriptionModel module)
        {
            script.AppendLine($"// module '{module.Name.ToCamelCase()}'");
            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine($"  CodeZero.services.{module.Name.ToCamelCase()} = CodeZero.services.{module.Name.ToCamelCase()} || {{}};");

            foreach (var controller in module.Controllers.Values)
            {
                script.AppendLine();
                AddControllerScript(script, module, controller);
            }

            script.AppendLine();
            script.AppendLine("})();");
        }

        private static void AddControllerScript(StringBuilder script, ModuleApiDescriptionModel module, ControllerApiDescriptionModel controller)
        {
            script.AppendLine($"  // controller '{controller.Name.ToCamelCase()}'");
            script.AppendLine("  (function(){");
            script.AppendLine();

            script.AppendLine($"    CodeZero.services.{module.Name.ToCamelCase()}.{controller.Name.ToCamelCase()} = CodeZero.services.{module.Name.ToCamelCase()}.{controller.Name.ToCamelCase()} || {{}};");

            foreach (var action in controller.Actions.Values)
            {
                script.AppendLine();
                AddActionScript(script, module, controller, action);
            }

            script.AppendLine();
            script.AppendLine("  })();");
        }

        private static void AddActionScript(StringBuilder script, ModuleApiDescriptionModel module, ControllerApiDescriptionModel controller, ActionApiDescriptionModel action)
        {
            var parameterList = ProxyScriptingJsFuncHelper.GenerateJsFuncParameterList(action, "ajaxParams");

            script.AppendLine($"    // action '{action.Name.ToCamelCase()}'");
            script.AppendLine($"    CodeZero.services.{module.Name.ToCamelCase()}.{controller.Name.ToCamelCase()}{ProxyScriptingJsFuncHelper.WrapWithBracketsOrWithDotPrefix(action.Name.ToCamelCase())} = function({parameterList}) {{");
            script.AppendLine("      return CodeZero.ajax($.extend(true, {");

            AddAjaxCallParameters(script, controller, action);

            script.AppendLine("      }, ajaxParams));;");
            script.AppendLine("    };");
        }

        private static void AddAjaxCallParameters(StringBuilder script, ControllerApiDescriptionModel controller, ActionApiDescriptionModel action)
        {
            var httpMethod = action.HttpMethod?.ToUpperInvariant() ?? "POST";

            script.AppendLine("        url: CodeZero.appPath + '" + ProxyScriptingHelper.GenerateUrlWithParameters(action) + "',");
            script.Append("        type: '" + httpMethod + "'");

            if (action.ReturnValue.Type == typeof(void))
            {
                script.AppendLine(",");
                script.Append("        dataType: null");
            }

            var headers = ProxyScriptingHelper.GenerateHeaders(action, 8);
            if (headers != null)
            {
                script.AppendLine(",");
                script.Append("        headers: " + headers);
            }

            var body = ProxyScriptingHelper.GenerateBody(action);
            if (!body.IsNullOrEmpty())
            {
                script.AppendLine(",");
                script.Append("        data: JSON.stringify(" + body + ")");
            }
            else
            {
                var formData = ProxyScriptingHelper.GenerateFormPostData(action, 8);
                if (!formData.IsNullOrEmpty())
                {
                    script.AppendLine(",");
                    script.Append("        data: " + formData);
                }
            }

            script.AppendLine();
        }
    }
}
