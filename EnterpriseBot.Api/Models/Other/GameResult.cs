using Newtonsoft.Json;

namespace EnterpriseBot.Api.Models.Other
{
    public class GameResult<T>
    {
        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonProperty("localizedError")]
        public LocalizedError LocalizedError { get; set; }

        public static implicit operator GameResult<T>(T type)
        {
            return new GameResult<T>
            {
                Result = type
            };
        }

        public static implicit operator GameResult<T>(LocalizedError localizedError)
        {
            return new GameResult<T>
            {
                LocalizedError = localizedError
            };
        }

        public static implicit operator T(GameResult<T> result)
        {
            return result.Result;
        }

        public static implicit operator EmptyGameResult(GameResult<T> result)
        {
            if (result.LocalizedError != null) return result.LocalizedError;
            return new EmptyGameResult();
        }

        //public static explicit operator T(GameResult<T> result)
        //{
        //    return result.Result;
        //}
    }
}
