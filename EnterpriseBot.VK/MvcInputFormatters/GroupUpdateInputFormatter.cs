using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using VkNet.Model.GroupUpdate;
using VkNet.Utils;

namespace EnterpriseBot.VK.MvcInputFormatters
{
    public class GroupUpdateInputFormatter : InputFormatter
    {
        public GroupUpdateInputFormatter()
        {
            SupportedMediaTypes.Add("application/json");
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            using (var reader = new StreamReader(request.Body))
            {
                string body = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(body))
                {
                    return InputFormatterResult.NoValue();
                }

                try
                {
                    var updateObject = JsonConvert.DeserializeObject(body);
                    var jtoken = JToken.FromObject(updateObject);

                    var update = GroupUpdate.FromJson(new VkResponse(jtoken));

                    return InputFormatterResult.Success(update);
                }
                catch
                {
                    return InputFormatterResult.Failure();
                }
            }
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(GroupUpdate);
        }
    }
}
