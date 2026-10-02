# Run on Windows after installing Ollama and pulling qwen2.5:3b.
param(
    [string]$Question,
    [string]$Model = 'qwen2.5:3b'
)
$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($Question)) {
    $Question = Read-Host 'Question and error text'
}
if ([string]::IsNullOrWhiteSpace($Question)) {
    throw 'Question is empty.'
}
$context = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'AI-CONTEXT.txt') -Raw -Encoding UTF8
$payload = @{
    model = $Model
    messages = @(
        @{ role = 'system'; content = $context },
        @{ role = 'user'; content = $Question }
    )
    stream = $false
} | ConvertTo-Json -Depth 6
try {
    $response = Invoke-RestMethod -Uri 'http://127.0.0.1:11434/api/chat' -Method Post -ContentType 'application/json; charset=utf-8' -Body ([System.Text.Encoding]::UTF8.GetBytes($payload)) -TimeoutSec 180
    $response.message.content
} catch {
    Write-Error ('Ollama request failed. Open Ollama, run: ollama pull ' + $Model + '. Details: ' + $_.Exception.Message)
}
