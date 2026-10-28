# Open in Notepad and paste blocks into Windows PowerShell.
# For NAT: host 80 -> guest 80. For host-only replace with the VM IP.
$ErrorActionPreference = 'Stop'
$base = 'http://127.0.0.1:80'

# Health and both collections
Invoke-RestMethod "$base/api/health"
Invoke-RestMethod "$base/api/products"
Invoke-RestMethod "$base/api/categories"

# Product: create -> read -> update -> read -> delete
$body = @{ name = 'Exam Monitor'; price = 299.99; stock = 5 } | ConvertTo-Json
$product = Invoke-RestMethod "$base/api/products" -Method Post -ContentType 'application/json' -Body $body
$product
$id = $product.id
Invoke-RestMethod "$base/api/products/$id"
$body = @{ name = 'Exam Monitor 4K'; price = 499.99; stock = 3 } | ConvertTo-Json
Invoke-RestMethod "$base/api/products/$id" -Method Put -ContentType 'application/json' -Body $body
Invoke-RestMethod "$base/api/products/$id"
Invoke-RestMethod "$base/api/products/$id" -Method Delete

# Category: create -> read -> update -> read -> delete
$body = @{ title = 'Exam'; description = 'Demo category' } | ConvertTo-Json
$category = Invoke-RestMethod "$base/api/categories" -Method Post -ContentType 'application/json' -Body $body
$category
$id = $category.id
Invoke-RestMethod "$base/api/categories/$id"
$body = @{ title = 'Exam updated'; description = 'Changed' } | ConvertTo-Json
Invoke-RestMethod "$base/api/categories/$id" -Method Put -ContentType 'application/json' -Body $body
Invoke-RestMethod "$base/api/categories/$id"
Invoke-RestMethod "$base/api/categories/$id" -Method Delete

# Expected: HTTP 200 for reads, 201 for create, 204 for update/delete.
# No printed body after successful PUT/DELETE is normal.
# This uses IDs returned by the server, never a hard-coded ID 4.
