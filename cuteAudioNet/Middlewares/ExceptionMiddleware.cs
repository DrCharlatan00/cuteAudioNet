using cuteAudioNet.Exceptions;
using FluentValidation;

namespace cuteAudioNet.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate requestDelegate, ILogger<object> logger) 
    {
        public async Task InvokeAsync(HttpContext context) {
            string? Function = null;
            string? CollectonOrItemName = null;
            Type? ClassType = null;
            Type? Item = null;
            Exception? exs = null;
            try
            {
                await requestDelegate(context);
            }
            catch (Exception ex) when (ex is IOperationException exception)
            {
                ClassType = exception.ClassThrow;
                Item = exception.Item;
                exs = ex;
            }
            catch (Exception ex) when (ex is IDbGetItemOrCollectionFailException exception) {
                Function = exception.NameFunction;
                CollectonOrItemName = exception.NameCollection;
            }
            catch (Exception ex) { exs = ex; }

            if (exs is not null) {
                string messageToLog = exs switch
                {
                    DbGetCollectionIsNull => $"Funcion {Function} throw expection broken collection this name or type or name class: {CollectonOrItemName}",
                    DbGetItemIsNull => $"Funcion {Function} throw expection bad item this name or type or name class: {CollectonOrItemName}",
                    CreateItemBaseFail<object, object> c => $"Create failed: class={ClassType.Name}, item={Item.Name}, msg={c.Message}",
                    UpdateItemBaseFail<object, object> u => $"Update failed: class={ClassType.Name}, item={Item.Name} msg={u.Message}",
                    RemoveItemBaseFail<object, object> r => $"Remove failed: class={ClassType.Name}, item={Item.Name}, msg={r.Message}",
                    _ => $"Operation failed: {exs.Message}"
                };
                logger.LogError(messageToLog);

                string MessageClient = exs switch
                {
                    DbGetCollectionIsNull => $"Can't return you data, collection is bad or not available",
                    DbGetItemIsNull => $"Can't return you data, item is bad or not available",
                    CreateItemBaseFail<object, object> => "Create is failed",
                    UpdateItemBaseFail<object, object> => $"Update is failed",
                    RemoveItemBaseFail<object, object> => $"Remove is failed",
                    ArgumentNullException c => $"{c.ParamName} is null or empty",
                    ValidationException d => $"Validation failed : {d.Errors} ",
                    NotImplementedException => "early)",
                    NullReferenceException => "Server stop operation whis truble null value",
                    _ => $"Operation finish whis failed"

                };

                var StatusCode = exs switch
                {
                    CreateItemBaseFail<object, object> => StatusCodes.Status400BadRequest,
                    UpdateItemBaseFail<object, object> => StatusCodes.Status400BadRequest,
                    RemoveItemBaseFail<object, object> => StatusCodes.Status400BadRequest,
                    ArgumentNullException => StatusCodes.Status400BadRequest,
                    ValidationException => StatusCodes.Status400BadRequest,
                    NotImplementedException => StatusCodes.Status501NotImplemented,
                    NullReferenceException => StatusCodes.Status503ServiceUnavailable,
                    _ => StatusCodes.Status500InternalServerError
                };
                context.Response.StatusCode = StatusCode;


                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = StatusCode,
                    Message = MessageClient
                });

            }


        }


        }

       

}

