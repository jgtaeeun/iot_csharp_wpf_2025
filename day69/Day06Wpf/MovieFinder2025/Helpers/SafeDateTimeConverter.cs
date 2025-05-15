using System.Text.Json;
using System.Text.Json.Serialization;

namespace MovieFinder2025.Helpers

{
    //json데이터를 api로 로드할 때, releasedate가 ""인 경우, 예외처리됨
    //범죄도시, 베테랑 등이 이 예외를 겪음
    public class SafeDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (string.IsNullOrWhiteSpace(str))
                    return null;  // 빈 문자열은 null로 처리

                if (DateTime.TryParse(str, out var date))
                    return date;  // 유효한 날짜 문자열을 DateTime으로 변환

                return null;  // 날짜 형식이 잘못된 경우 null 반환
            }

            return null;  // 다른 타입의 경우 null 반환
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
            else
                writer.WriteNullValue();  // null 값은 "null"로 출력
        }
    }
}