using SlackCustomStyle;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "slack", "app-4.29.149");
var fileName = "slack.exe";
var port = 9222;

//실행 중인 슬랙 종료
Process.GetProcessesByName("slack").ToList().ForEach(p => p.Kill());

//슬랙 실행
var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = fileName,
        WorkingDirectory = path,
        Arguments = $"--remote-debugging-port={port}",
        UseShellExecute = true
    }
};
process.Start();

//디버그 정보 조회
var client = new HttpClient();
var response = await client.GetAsync($"http://localhost:{port}/json/list");
var debugInfo = await response.Content.ReadFromJsonAsync<List<DebugInfo>>();
var pageDebugInfo = debugInfo.Where(x => x.Type == "page").FirstOrDefault();

//스크립트 전송
using (var ws = new ClientWebSocket())
{
    await ws.ConnectAsync(new Uri(pageDebugInfo.WebSocketDebuggerUrl), CancellationToken.None);

    var encoderSettings = new TextEncoderSettings();
    encoderSettings.AllowCharacters('\u0436', '\u0430', '\u0027');
    encoderSettings.AllowRanges(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);

    var options = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };

    var message = JsonSerializer.Serialize(new
    {
        id = 2,
        method = "Runtime.evaluate",
        @params = new
        {
            expression = @$"

                var style = document.createElement('style');
                style.innerHTML = `
                    {File.ReadAllText("style.css", Encoding.UTF8)}
                `;
                document.head.appendChild(style);

            "
        }
    }, options);

    var buffer = Encoding.UTF8.GetBytes(message);
    await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
}

Console.ReadLine();