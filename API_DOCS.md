---
title: EMarket API v1
language_tabs:
  - shell: Shell
  - http: HTTP
  - javascript: JavaScript
  - ruby: Ruby
  - python: Python
  - php: PHP
  - java: Java
  - go: Go
toc_footers: []
includes: []
search: true
highlight_theme: darkula
headingLevel: 2

---

<!-- Generator: Widdershins v4.0.1 -->

<h1 id="emarket-api">EMarket API v1</h1>

> Scroll down for code samples, example requests and responses. Select a language for code samples from the tabs above or the mobile navigation menu.

Base URLs:

* <a href="https://localhost:44339">https://localhost:44339</a>

<h1 id="emarket-api-aianalysisapi">AIAnalysisApi</h1>

## Lấy danh sách gợi ý nhập hàng chi tiết do AI tính toán (Replenishment Advice).
Bao gồm: Tồn kho hiện tại, Nhu cầu dự báo, Số lượng đề xuất nhập, Lý do.

<a id="opIdAIAnalysisApi_GetReplenishmentAdvice"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/replenishment-advice/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/replenishment-advice/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/replenishment-advice/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/replenishment-advice/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/replenishment-advice/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/replenishment-advice/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/replenishment-advice/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/replenishment-advice/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/replenishment-advice/{branchId}`

<h3 id="lấy-danh-sách-gợi-ý-nhập-hàng-chi-tiết-do-ai-tính-toán-(replenishment-advice).
bao-gồm:-tồn-kho-hiện-tại,-nhu-cầu-dự-báo,-số-lượng-đề-xuất-nhập,-lý-do.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-gợi-ý-nhập-hàng-chi-tiết-do-ai-tính-toán-(replenishment-advice).
bao-gồm:-tồn-kho-hiện-tại,-nhu-cầu-dự-báo,-số-lượng-đề-xuất-nhập,-lý-do.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-gợi-ý-nhập-hàng-chi-tiết-do-ai-tính-toán-(replenishment-advice).
bao-gồm:-tồn-kho-hiện-tại,-nhu-cầu-dự-báo,-số-lượng-đề-xuất-nhập,-lý-do.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách gợi ý nhập hàng dạng Recommendation (Join tên sản phẩm + danh mục).

<a id="opIdAIAnalysisApi_GetRecommendations"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/recommendations/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/recommendations/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/recommendations/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/recommendations/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/recommendations/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/recommendations/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/recommendations/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/recommendations/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/recommendations/{branchId}`

<h3 id="lấy-danh-sách-gợi-ý-nhập-hàng-dạng-recommendation-(join-tên-sản-phẩm-+-danh-mục).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-gợi-ý-nhập-hàng-dạng-recommendation-(join-tên-sản-phẩm-+-danh-mục).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-gợi-ý-nhập-hàng-dạng-recommendation-(join-tên-sản-phẩm-+-danh-mục).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách bất thường ngành hàng (Spike/Dip) do AI phát hiện.

<a id="opIdAIAnalysisApi_GetAnomalies"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/anomalies/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/anomalies/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/anomalies/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/anomalies/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/anomalies/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/anomalies/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/anomalies/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/anomalies/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/anomalies/{branchId}`

<h3 id="lấy-danh-sách-bất-thường-ngành-hàng-(spike/dip)-do-ai-phát-hiện.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-bất-thường-ngành-hàng-(spike/dip)-do-ai-phát-hiện.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-bất-thường-ngành-hàng-(spike/dip)-do-ai-phát-hiện.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy insight chi tiết sản phẩm: Star, Trending, Warning.

<a id="opIdAIAnalysisApi_GetProductInsights"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/insights/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/insights/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/insights/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/insights/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/insights/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/insights/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/insights/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/insights/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/insights/{branchId}`

<h3 id="lấy-insight-chi-tiết-sản-phẩm:-star,-trending,-warning.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-insight-chi-tiết-sản-phẩm:-star,-trending,-warning.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-insight-chi-tiết-sản-phẩm:-star,-trending,-warning.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy dữ liệu dự báo tồn kho (Inventory Forecast) cho toàn bộ sản phẩm tại chi nhánh.

<a id="opIdAIAnalysisApi_GetInventoryForecast"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/inventory-forecast/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/inventory-forecast/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/inventory-forecast/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/inventory-forecast/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/inventory-forecast/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/inventory-forecast/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/inventory-forecast/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/inventory-forecast/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/inventory-forecast/{branchId}`

<h3 id="lấy-dữ-liệu-dự-báo-tồn-kho-(inventory-forecast)-cho-toàn-bộ-sản-phẩm-tại-chi-nhánh.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-dữ-liệu-dự-báo-tồn-kho-(inventory-forecast)-cho-toàn-bộ-sản-phẩm-tại-chi-nhánh.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-dữ-liệu-dự-báo-tồn-kho-(inventory-forecast)-cho-toàn-bộ-sản-phẩm-tại-chi-nhánh.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy dữ liệu dự báo bán hàng (Sales Forecast) theo chuỗi thời gian cho một sản phẩm cụ thể.

<a id="opIdAIAnalysisApi_GetSalesForecast"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/sales-forecast/{productId}/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/sales-forecast/{productId}/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/sales-forecast/{productId}/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/sales-forecast/{productId}/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/sales-forecast/{productId}/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/sales-forecast/{productId}/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/sales-forecast/{productId}/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/sales-forecast/{productId}/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/sales-forecast/{productId}/{branchId}`

<h3 id="lấy-dữ-liệu-dự-báo-bán-hàng-(sales-forecast)-theo-chuỗi-thời-gian-cho-một-sản-phẩm-cụ-thể.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|productId|path|integer(int32)|true|none|
|branchId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-dữ-liệu-dự-báo-bán-hàng-(sales-forecast)-theo-chuỗi-thời-gian-cho-một-sản-phẩm-cụ-thể.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-dữ-liệu-dự-báo-bán-hàng-(sales-forecast)-theo-chuỗi-thời-gian-cho-một-sản-phẩm-cụ-thể.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy Top N sản phẩm có sản lượng dự báo cao nhất tại chi nhánh.

<a id="opIdAIAnalysisApi_GetTopPredictedProducts"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/top-predicted/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/top-predicted/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/top-predicted/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/top-predicted/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/top-predicted/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/top-predicted/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/top-predicted/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/top-predicted/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/top-predicted/{branchId}`

<h3 id="lấy-top-n-sản-phẩm-có-sản-lượng-dự-báo-cao-nhất-tại-chi-nhánh.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|path|integer(int32)|true|none|
|topCount|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-top-n-sản-phẩm-có-sản-lượng-dự-báo-cao-nhất-tại-chi-nhánh.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-top-n-sản-phẩm-có-sản-lượng-dự-báo-cao-nhất-tại-chi-nhánh.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách hàng tồn kho chậm luân chuyển (Deadstock) có nguy cơ hết hạn hoặc ứ đọng.

<a id="opIdAIAnalysisApi_GetDeadstockAnalysis"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/deadstock/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/deadstock/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/deadstock/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/deadstock/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/deadstock/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/deadstock/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/deadstock/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/deadstock/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/deadstock/{branchId}`

<h3 id="lấy-danh-sách-hàng-tồn-kho-chậm-luân-chuyển-(deadstock)-có-nguy-cơ-hết-hạn-hoặc-ứ-đọng.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-hàng-tồn-kho-chậm-luân-chuyển-(deadstock)-có-nguy-cơ-hết-hạn-hoặc-ứ-đọng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-hàng-tồn-kho-chậm-luân-chuyển-(deadstock)-có-nguy-cơ-hết-hạn-hoặc-ứ-đọng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy chuỗi thời gian lịch sử bán hàng theo ngày cho một sản phẩm cụ thể.
Trả về danh sách {Date, Qty} dùng để vẽ biểu đồ xu hướng.

<a id="opIdAIAnalysisApi_GetProductHistory"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/product-history/{productId}/{branchId}?start=string&end=string \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/product-history/{productId}/{branchId}?start=string&end=string HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/product-history/{productId}/{branchId}?start=string&end=string',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/product-history/{productId}/{branchId}',
  params: {
  'start' => 'string',
'end' => 'string'
}, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/product-history/{productId}/{branchId}', params={
  'start': 'string',  'end': 'string'
}, headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/product-history/{productId}/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/product-history/{productId}/{branchId}?start=string&end=string");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/product-history/{productId}/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/product-history/{productId}/{branchId}`

<h3 id="lấy-chuỗi-thời-gian-lịch-sử-bán-hàng-theo-ngày-cho-một-sản-phẩm-cụ-thể.
trả-về-danh-sách-{date,-qty}-dùng-để-vẽ-biểu-đồ-xu-hướng.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|productId|path|integer(int32)|true|none|
|branchId|path|integer(int32)|true|none|
|start|query|string|true|none|
|end|query|string|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-chuỗi-thời-gian-lịch-sử-bán-hàng-theo-ngày-cho-một-sản-phẩm-cụ-thể.
trả-về-danh-sách-{date,-qty}-dùng-để-vẽ-biểu-đồ-xu-hướng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-chuỗi-thời-gian-lịch-sử-bán-hàng-theo-ngày-cho-một-sản-phẩm-cụ-thể.
trả-về-danh-sách-{date,-qty}-dùng-để-vẽ-biểu-đồ-xu-hướng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy phân tích rủi ro tài chính theo lô hàng (FEFO - First Expired First Out).
Bao gồm: Tổng giá trị dự phòng, Số lô nguy hiểm, Chi tiết từng lô.

<a id="opIdAIAnalysisApi_GetLotFinancialRisk"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/ai-analysis/lot-financial-risk/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/ai-analysis/lot-financial-risk/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/lot-financial-risk/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/ai-analysis/lot-financial-risk/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/ai-analysis/lot-financial-risk/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/ai-analysis/lot-financial-risk/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/lot-financial-risk/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/ai-analysis/lot-financial-risk/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/ai-analysis/lot-financial-risk/{branchId}`

<h3 id="lấy-phân-tích-rủi-ro-tài-chính-theo-lô-hàng-(fefo---first-expired-first-out).
bao-gồm:-tổng-giá-trị-dự-phòng,-số-lô-nguy-hiểm,-chi-tiết-từng-lô.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-phân-tích-rủi-ro-tài-chính-theo-lô-hàng-(fefo---first-expired-first-out).
bao-gồm:-tổng-giá-trị-dự-phòng,-số-lô-nguy-hiểm,-chi-tiết-từng-lô.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-phân-tích-rủi-ro-tài-chính-theo-lô-hàng-(fefo---first-expired-first-out).
bao-gồm:-tổng-giá-trị-dự-phòng,-số-lô-nguy-hiểm,-chi-tiết-từng-lô.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Kích hoạt quy trình huấn luyện AI Python Pipeline. CHỈ dùng khi có yêu cầu rõ ràng.

<a id="opIdAIAnalysisApi_RunProphet"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/ai-analysis/run-prophet \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/ai-analysis/run-prophet HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/run-prophet',
{
  method: 'POST',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/ai-analysis/run-prophet',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/ai-analysis/run-prophet', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/ai-analysis/run-prophet', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/run-prophet");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/ai-analysis/run-prophet", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/ai-analysis/run-prophet`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="kích-hoạt-quy-trình-huấn-luyện-ai-python-pipeline.-chỉ-dùng-khi-có-yêu-cầu-rõ-ràng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="kích-hoạt-quy-trình-huấn-luyện-ai-python-pipeline.-chỉ-dùng-khi-có-yêu-cầu-rõ-ràng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Kích hoạt chạy phân tích (mode = "system" cho Rule-based, hoặc "ai" cho Python/XGBoost).

<a id="opIdAIAnalysisApi_RunAnalysis"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/ai-analysis/run-analysis \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/ai-analysis/run-analysis HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "Mode": "string",
  "BranchId": 0
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/ai-analysis/run-analysis',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/ai-analysis/run-analysis',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/ai-analysis/run-analysis', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/ai-analysis/run-analysis', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/ai-analysis/run-analysis");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/ai-analysis/run-analysis", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/ai-analysis/run-analysis`

> Body parameter

```json
{
  "Mode": "string",
  "BranchId": 0
}
```

```
Mode: string
BranchId: 0

```

```yaml
Mode: string
BranchId: 0

```

<h3 id="kích-hoạt-chạy-phân-tích-(mode-=-"system"-cho-rule-based,-hoặc-"ai"-cho-python/xgboost).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[RunAnalysisRequest](#schemarunanalysisrequest)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="kích-hoạt-chạy-phân-tích-(mode-=-"system"-cho-rule-based,-hoặc-"ai"-cho-python/xgboost).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="kích-hoạt-chạy-phân-tích-(mode-=-"system"-cho-rule-based,-hoặc-"ai"-cho-python/xgboost).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-customeradminapi">CustomerAdminApi</h1>

## CustomerAdminApi_GetAllCustomers

<a id="opIdCustomerAdminApi_GetAllCustomers"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getallcustomers-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getallcustomers-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_SearchCustomers

<a id="opIdCustomerAdminApi_SearchCustomers"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/paged \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/paged HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/paged',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/paged',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/paged', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/paged', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/paged");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/paged", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/paged`

<h3 id="customeradminapi_searchcustomers-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|keyword|query|string|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_searchcustomers-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_searchcustomers-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetCustomerById

<a id="opIdCustomerAdminApi_GetCustomerById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/{id}`

<h3 id="customeradminapi_getcustomerbyid-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getcustomerbyid-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getcustomerbyid-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_UpdateCustomer

<a id="opIdCustomerAdminApi_UpdateCustomer"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/customer/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/customer/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "CustomerId": 0,
  "FullName": "string",
  "Email": "string",
  "Phone": "string",
  "CustomerType": "string",
  "UserImg": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/customer/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/customer/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/customer/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/customer/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/customer/update`

> Body parameter

```json
{
  "CustomerId": 0,
  "FullName": "string",
  "Email": "string",
  "Phone": "string",
  "CustomerType": "string",
  "UserImg": "string"
}
```

```
CustomerId: 0
FullName: string
Email: string
Phone: string
CustomerType: string
UserImg: string

```

```yaml
CustomerId: 0
FullName: string
Email: string
Phone: string
CustomerType: string
UserImg: string

```

<h3 id="customeradminapi_updatecustomer-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|query|integer(int32)|false|none|
|body|body|[CustomerUpdateDTO](#schemacustomerupdatedto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_updatecustomer-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_updatecustomer-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_DeleteCustomer

<a id="opIdCustomerAdminApi_DeleteCustomer"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/customer/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/customer/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/customer/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/customer/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/customer/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/customer/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/customer/delete/{id}`

<h3 id="customeradminapi_deletecustomer-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_deletecustomer-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_deletecustomer-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetCustomerEmail

<a id="opIdCustomerAdminApi_GetCustomerEmail"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/{id}/email \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/{id}/email HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/{id}/email',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/{id}/email',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/{id}/email', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/{id}/email', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/{id}/email");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/{id}/email", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/{id}/email`

<h3 id="customeradminapi_getcustomeremail-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getcustomeremail-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getcustomeremail-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_CreateCustomer

<a id="opIdCustomerAdminApi_CreateCustomer"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/customer/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/customer/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "FullName": "string",
  "Email": "string",
  "Phone": "string",
  "CustomerType": "string",
  "UserImg": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/customer/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/customer/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/customer/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/customer/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/customer/create`

> Body parameter

```json
{
  "FullName": "string",
  "Email": "string",
  "Phone": "string",
  "CustomerType": "string",
  "UserImg": "string"
}
```

```
FullName: string
Email: string
Phone: string
CustomerType: string
UserImg: string

```

```yaml
FullName: string
Email: string
Phone: string
CustomerType: string
UserImg: string

```

<h3 id="customeradminapi_createcustomer-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[CustomerCreateDTO](#schemacustomercreatedto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_createcustomer-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_createcustomer-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetCustomerStats

<a id="opIdCustomerAdminApi_GetCustomerStats"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/stats \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/stats HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/stats',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/stats',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/stats', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/stats', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/stats");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/stats", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/stats`

<h3 id="customeradminapi_getcustomerstats-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|fromDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getcustomerstats-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getcustomerstats-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_CountCreatedInMonth

<a id="opIdCustomerAdminApi_CountCreatedInMonth"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/count-in-month?fromDate=2019-08-24T14%3A15%3A22Z&toDate=2019-08-24T14%3A15%3A22Z \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/count-in-month?fromDate=2019-08-24T14%3A15%3A22Z&toDate=2019-08-24T14%3A15%3A22Z HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/count-in-month?fromDate=2019-08-24T14%3A15%3A22Z&toDate=2019-08-24T14%3A15%3A22Z',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/count-in-month',
  params: {
  'fromDate' => 'string(date-time)',
'toDate' => 'string(date-time)'
}, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/count-in-month', params={
  'fromDate': '2019-08-24T14:15:22Z',  'toDate': '2019-08-24T14:15:22Z'
}, headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/count-in-month', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/count-in-month?fromDate=2019-08-24T14%3A15%3A22Z&toDate=2019-08-24T14%3A15%3A22Z");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/count-in-month", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/count-in-month`

<h3 id="customeradminapi_countcreatedinmonth-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|fromDate|query|string(date-time)|true|none|
|toDate|query|string(date-time)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_countcreatedinmonth-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_countcreatedinmonth-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetCustomerSegments

<a id="opIdCustomerAdminApi_GetCustomerSegments"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/segments \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/segments HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/segments',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/segments',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/segments', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/segments', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/segments");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/segments", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/segments`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getcustomersegments-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getcustomersegments-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetCustomersCreatedByMonth

<a id="opIdCustomerAdminApi_GetCustomersCreatedByMonth"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/created-by-month \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/created-by-month HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/created-by-month',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/created-by-month',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/created-by-month', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/created-by-month', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/created-by-month");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/created-by-month", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/created-by-month`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getcustomerscreatedbymonth-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getcustomerscreatedbymonth-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetTopCustomers

<a id="opIdCustomerAdminApi_GetTopCustomers"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/top \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/top HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/top',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/top',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/top', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/top', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/top");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/top", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/top`

<h3 id="customeradminapi_gettopcustomers-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|top|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_gettopcustomers-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_gettopcustomers-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetAddressesByCustomer

<a id="opIdCustomerAdminApi_GetAddressesByCustomer"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/address/by-customer/{customerId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/address/by-customer/{customerId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/address/by-customer/{customerId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/address/by-customer/{customerId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/address/by-customer/{customerId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/address/by-customer/{customerId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/address/by-customer/{customerId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/address/by-customer/{customerId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/address/by-customer/{customerId}`

<h3 id="customeradminapi_getaddressesbycustomer-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|customerId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getaddressesbycustomer-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getaddressesbycustomer-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetDefaultAddress

<a id="opIdCustomerAdminApi_GetDefaultAddress"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/address/default/{customerId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/address/default/{customerId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/address/default/{customerId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/address/default/{customerId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/address/default/{customerId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/address/default/{customerId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/address/default/{customerId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/address/default/{customerId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/address/default/{customerId}`

<h3 id="customeradminapi_getdefaultaddress-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|customerId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getdefaultaddress-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getdefaultaddress-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetAddressById

<a id="opIdCustomerAdminApi_GetAddressById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/address/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/address/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/address/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/address/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/address/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/address/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/address/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/address/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/address/{id}`

<h3 id="customeradminapi_getaddressbyid-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getaddressbyid-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getaddressbyid-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_UpdateAddress

<a id="opIdCustomerAdminApi_UpdateAddress"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/customer/address/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/customer/address/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "AddressId": 0,
  "CustomerId": 0,
  "FullAddress": "string",
  "AddressUrl": "string",
  "IsDefault": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/address/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/customer/address/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/customer/address/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/customer/address/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/address/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/customer/address/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/customer/address/update`

> Body parameter

```json
{
  "AddressId": 0,
  "CustomerId": 0,
  "FullAddress": "string",
  "AddressUrl": "string",
  "IsDefault": true
}
```

```
AddressId: 0
CustomerId: 0
FullAddress: string
AddressUrl: string
IsDefault: true

```

```yaml
AddressId: 0
CustomerId: 0
FullAddress: string
AddressUrl: string
IsDefault: true

```

<h3 id="customeradminapi_updateaddress-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|query|integer(int32)|false|none|
|body|body|[CustomerAddressUpdateDTO](#schemacustomeraddressupdatedto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_updateaddress-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_updateaddress-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_DeleteAddress

<a id="opIdCustomerAdminApi_DeleteAddress"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/customer/address/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/customer/address/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/address/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/customer/address/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/customer/address/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/customer/address/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/address/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/customer/address/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/customer/address/delete/{id}`

<h3 id="customeradminapi_deleteaddress-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_deleteaddress-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_deleteaddress-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_CreateAddress

<a id="opIdCustomerAdminApi_CreateAddress"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/customer/address/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/customer/address/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "CustomerId": 0,
  "FullAddress": "string",
  "AddressUrl": "string",
  "IsDefault": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/address/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/customer/address/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/customer/address/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/customer/address/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/address/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/customer/address/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/customer/address/create`

> Body parameter

```json
{
  "CustomerId": 0,
  "FullAddress": "string",
  "AddressUrl": "string",
  "IsDefault": true
}
```

```
CustomerId: 0
FullAddress: string
AddressUrl: string
IsDefault: true

```

```yaml
CustomerId: 0
FullAddress: string
AddressUrl: string
IsDefault: true

```

<h3 id="customeradminapi_createaddress-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[CustomerAddressCreateDTO](#schemacustomeraddresscreatedto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_createaddress-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_createaddress-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetAllLoyalty

<a id="opIdCustomerAdminApi_GetAllLoyalty"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/loyalty \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/loyalty HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/loyalty',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/loyalty',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/loyalty', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/loyalty', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/loyalty");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/loyalty", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/loyalty`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getallloyalty-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getallloyalty-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_GetLoyaltyById

<a id="opIdCustomerAdminApi_GetLoyaltyById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/customer/loyalty/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/customer/loyalty/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/loyalty/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/customer/loyalty/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/customer/loyalty/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/customer/loyalty/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/loyalty/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/customer/loyalty/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/customer/loyalty/{id}`

<h3 id="customeradminapi_getloyaltybyid-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_getloyaltybyid-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_getloyaltybyid-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_UpdateLoyalty

<a id="opIdCustomerAdminApi_UpdateLoyalty"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/customer/loyalty/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/customer/loyalty/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "LoyaltyId": 0,
  "CustomerId": 0,
  "OrderId": 0,
  "PointsEarned": 0,
  "PointsRedeemed": 0,
  "CreatedAt": "2019-08-24T14:15:22Z"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/loyalty/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/customer/loyalty/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/customer/loyalty/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/customer/loyalty/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/loyalty/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/customer/loyalty/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/customer/loyalty/update`

> Body parameter

```json
{
  "LoyaltyId": 0,
  "CustomerId": 0,
  "OrderId": 0,
  "PointsEarned": 0,
  "PointsRedeemed": 0,
  "CreatedAt": "2019-08-24T14:15:22Z"
}
```

```
LoyaltyId: 0
CustomerId: 0
OrderId: 0
PointsEarned: 0
PointsRedeemed: 0
CreatedAt: 2019-08-24T14:15:22Z

```

```yaml
LoyaltyId: 0
CustomerId: 0
OrderId: 0
PointsEarned: 0
PointsRedeemed: 0
CreatedAt: 2019-08-24T14:15:22Z

```

<h3 id="customeradminapi_updateloyalty-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|query|integer(int32)|false|none|
|body|body|[LoyaltyProgramDTO](#schemaloyaltyprogramdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_updateloyalty-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_updateloyalty-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_DeleteLoyalty

<a id="opIdCustomerAdminApi_DeleteLoyalty"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/customer/loyalty/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/customer/loyalty/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/loyalty/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/customer/loyalty/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/customer/loyalty/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/customer/loyalty/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/loyalty/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/customer/loyalty/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/customer/loyalty/delete/{id}`

<h3 id="customeradminapi_deleteloyalty-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_deleteloyalty-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_deleteloyalty-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## CustomerAdminApi_CreateLoyalty

<a id="opIdCustomerAdminApi_CreateLoyalty"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/customer/loyalty/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/customer/loyalty/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "LoyaltyId": 0,
  "CustomerId": 0,
  "OrderId": 0,
  "PointsEarned": 0,
  "PointsRedeemed": 0,
  "CreatedAt": "2019-08-24T14:15:22Z"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/customer/loyalty/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/customer/loyalty/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/customer/loyalty/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/customer/loyalty/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/customer/loyalty/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/customer/loyalty/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/customer/loyalty/create`

> Body parameter

```json
{
  "LoyaltyId": 0,
  "CustomerId": 0,
  "OrderId": 0,
  "PointsEarned": 0,
  "PointsRedeemed": 0,
  "CreatedAt": "2019-08-24T14:15:22Z"
}
```

```
LoyaltyId: 0
CustomerId: 0
OrderId: 0
PointsEarned: 0
PointsRedeemed: 0
CreatedAt: 2019-08-24T14:15:22Z

```

```yaml
LoyaltyId: 0
CustomerId: 0
OrderId: 0
PointsEarned: 0
PointsRedeemed: 0
CreatedAt: 2019-08-24T14:15:22Z

```

<h3 id="customeradminapi_createloyalty-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[LoyaltyProgramDTO](#schemaloyaltyprogramdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="customeradminapi_createloyalty-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="customeradminapi_createloyalty-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-dashboardadminapi">DashboardAdminApi</h1>

## Lấy tổng quan dashboard: doanh thu, khách hàng, sản phẩm, tồn kho.

<a id="opIdDashboardAdminApi_GetSummary"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/summary?fromDate=2019-08-24T14%3A15%3A22Z&toDate=2019-08-24T14%3A15%3A22Z&groupBy=string \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/summary?fromDate=2019-08-24T14%3A15%3A22Z&toDate=2019-08-24T14%3A15%3A22Z&groupBy=string HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/summary?fromDate=2019-08-24T14%3A15%3A22Z&toDate=2019-08-24T14%3A15%3A22Z&groupBy=string',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/summary',
  params: {
  'fromDate' => 'string(date-time)',
'toDate' => 'string(date-time)',
'groupBy' => 'string'
}, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/summary', params={
  'fromDate': '2019-08-24T14:15:22Z',  'toDate': '2019-08-24T14:15:22Z',  'groupBy': 'string'
}, headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/summary', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/summary?fromDate=2019-08-24T14%3A15%3A22Z&toDate=2019-08-24T14%3A15%3A22Z&groupBy=string");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/summary", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/summary`

<h3 id="lấy-tổng-quan-dashboard:-doanh-thu,-khách-hàng,-sản-phẩm,-tồn-kho.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|fromDate|query|string(date-time)|true|none|
|toDate|query|string(date-time)|true|none|
|groupBy|query|string|true|none|
|branchId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tổng-quan-dashboard:-doanh-thu,-khách-hàng,-sản-phẩm,-tồn-kho.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tổng-quan-dashboard:-doanh-thu,-khách-hàng,-sản-phẩm,-tồn-kho.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy thống kê KPI tổng quát (Doanh thu hôm nay/tháng, số đơn hàng, khách hàng, cảnh báo tồn kho).

<a id="opIdDashboardAdminApi_GetKpiSummary"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/cards \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/cards HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/cards',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/cards',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/cards', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/cards', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/cards");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/cards", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/cards`

<h3 id="lấy-thống-kê-kpi-tổng-quát-(doanh-thu-hôm-nay/tháng,-số-đơn-hàng,-khách-hàng,-cảnh-báo-tồn-kho).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-thống-kê-kpi-tổng-quát-(doanh-thu-hôm-nay/tháng,-số-đơn-hàng,-khách-hàng,-cảnh-báo-tồn-kho).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-thống-kê-kpi-tổng-quát-(doanh-thu-hôm-nay/tháng,-số-đơn-hàng,-khách-hàng,-cảnh-báo-tồn-kho).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy hiệu suất chi nhánh theo khoảng thời gian.

<a id="opIdDashboardAdminApi_GetBranchPerformance"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/branch-performance \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/branch-performance HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/branch-performance',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/branch-performance',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/branch-performance', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/branch-performance', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/branch-performance");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/branch-performance", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/branch-performance`

<h3 id="lấy-hiệu-suất-chi-nhánh-theo-khoảng-thời-gian.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|query|integer(int32)|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-hiệu-suất-chi-nhánh-theo-khoảng-thời-gian.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-hiệu-suất-chi-nhánh-theo-khoảng-thời-gian.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy dữ liệu biểu đồ tồn kho theo chi nhánh.

<a id="opIdDashboardAdminApi_GetStockChart"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/stock-chart \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/stock-chart HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/stock-chart',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/stock-chart',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/stock-chart', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/stock-chart', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/stock-chart");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/stock-chart", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/stock-chart`

<h3 id="lấy-dữ-liệu-biểu-đồ-tồn-kho-theo-chi-nhánh.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-dữ-liệu-biểu-đồ-tồn-kho-theo-chi-nhánh.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-dữ-liệu-biểu-đồ-tồn-kho-theo-chi-nhánh.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy dữ liệu tổng quan theo ngày hoặc tháng (Revenue, Orders, Customers...).

<a id="opIdDashboardAdminApi_GetOverview"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/overview \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/overview HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/overview',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/overview',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/overview', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/overview', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/overview");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/overview", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/overview`

<h3 id="lấy-dữ-liệu-tổng-quan-theo-ngày-hoặc-tháng-(revenue,-orders,-customers...).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|query|integer(int32)|false|ID chi nhánh.|
|fromDate|query|string(date-time)|false|Ngày bắt đầu.|
|toDate|query|string(date-time)|false|Ngày kết thúc.|
|groupBy|query|string|false|"day" hoặc "month".|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-dữ-liệu-tổng-quan-theo-ngày-hoặc-tháng-(revenue,-orders,-customers...).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-dữ-liệu-tổng-quan-theo-ngày-hoặc-tháng-(revenue,-orders,-customers...).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy dashboard nhân sự: nhân viên, vai trò, tăng trưởng.

<a id="opIdDashboardAdminApi_GetPeopleDashboard"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/people \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/people HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/people',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/people',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/people', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/people', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/people");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/people", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/people`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-dashboard-nhân-sự:-nhân-viên,-vai-trò,-tăng-trưởng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-dashboard-nhân-sự:-nhân-viên,-vai-trò,-tăng-trưởng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy dashboard kho: nhập - xuất - tồn, biến động.

<a id="opIdDashboardAdminApi_GetWarehouseDashboard"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/warehouse \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/warehouse HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/warehouse',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/warehouse',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/warehouse', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/warehouse', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/warehouse");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/warehouse", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/warehouse`

<h3 id="lấy-dashboard-kho:-nhập---xuất---tồn,-biến-động.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|dayBacks|query|integer(int32)|false|none|
|branchId|query|integer(int32)|false|none|
|warehouseId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-dashboard-kho:-nhập---xuất---tồn,-biến-động.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-dashboard-kho:-nhập---xuất---tồn,-biến-động.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy dashboard tài chính: doanh thu, lợi nhuận, dòng tiền.

<a id="opIdDashboardAdminApi_GetFinanceDashboard"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/finance \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/finance HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/finance',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/finance',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/finance', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/finance', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/finance");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/finance", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/finance`

<h3 id="lấy-dashboard-tài-chính:-doanh-thu,-lợi-nhuận,-dòng-tiền.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|daysBack|query|integer(int32)|false|none|
|branchId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-dashboard-tài-chính:-doanh-thu,-lợi-nhuận,-dòng-tiền.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-dashboard-tài-chính:-doanh-thu,-lợi-nhuận,-dòng-tiền.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy dashboard công nợ nhà cung cấp với bộ lọc chi nhánh, NCC, thời gian.

<a id="opIdDashboardAdminApi_GetDebtDashboard"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/debt \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/debt HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/debt',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/debt',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/debt', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/debt', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/debt");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/debt", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/debt`

<h3 id="lấy-dashboard-công-nợ-nhà-cung-cấp-với-bộ-lọc-chi-nhánh,-ncc,-thời-gian.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|query|integer(int32)|false|none|
|supplierId|query|integer(int32)|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-dashboard-công-nợ-nhà-cung-cấp-với-bộ-lọc-chi-nhánh,-ncc,-thời-gian.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-dashboard-công-nợ-nhà-cung-cấp-với-bộ-lọc-chi-nhánh,-ncc,-thời-gian.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy dữ liệu tổng hợp dành riêng cho Super Admin (toàn hệ thống).

<a id="opIdDashboardAdminApi_GetSuperAdminHub"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/dashboard/super-admin \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/dashboard/super-admin HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/dashboard/super-admin',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/dashboard/super-admin',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/dashboard/super-admin', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/dashboard/super-admin', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/dashboard/super-admin");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/dashboard/super-admin", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/dashboard/super-admin`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-dữ-liệu-tổng-hợp-dành-riêng-cho-super-admin-(toàn-hệ-thống).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-dữ-liệu-tổng-hợp-dành-riêng-cho-super-admin-(toàn-hệ-thống).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-expenseadminapi">ExpenseAdminApi</h1>

## ExpenseAdminApi_GetExpenses

<a id="opIdExpenseAdminApi_GetExpenses"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/expense/list \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/expense/list HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/list',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/expense/list',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/expense/list', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/expense/list', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/list");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/expense/list", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/expense/list`

<h3 id="expenseadminapi_getexpenses-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|query|integer(int32)|false|none|
|categoryId|query|integer(int32)|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|
|status|query|string|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_getexpenses-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_getexpenses-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách phiếu chi có phân trang.

<a id="opIdExpenseAdminApi_GetExpensesPaged"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/expense/paged \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/expense/paged HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/paged',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/expense/paged',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/expense/paged', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/expense/paged', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/paged");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/expense/paged", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/expense/paged`

<h3 id="lấy-danh-sách-phiếu-chi-có-phân-trang.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|branchId|query|integer(int32)|false|none|
|categoryId|query|integer(int32)|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|
|status|query|string|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-phiếu-chi-có-phân-trang.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-phiếu-chi-có-phân-trang.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Thống kê tổng hợp chi phí theo trạng thái &amp; tổng tiền.

<a id="opIdExpenseAdminApi_GetExpenseSummary"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/expense/summary \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/expense/summary HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/summary',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/expense/summary',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/expense/summary', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/expense/summary', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/summary");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/expense/summary", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/expense/summary`

<h3 id="thống-kê-tổng-hợp-chi-phí-theo-trạng-thái-&amp;-tổng-tiền.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|query|integer(int32)|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="thống-kê-tổng-hợp-chi-phí-theo-trạng-thái-&amp;-tổng-tiền.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="thống-kê-tổng-hợp-chi-phí-theo-trạng-thái-&amp;-tổng-tiền.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_GetExpenseDetail

<a id="opIdExpenseAdminApi_GetExpenseDetail"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/expense/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/expense/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/expense/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/expense/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/expense/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/expense/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/expense/{id}`

<h3 id="expenseadminapi_getexpensedetail-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_getexpensedetail-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_getexpensedetail-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_DeleteExpense

<a id="opIdExpenseAdminApi_DeleteExpense"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/expense/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/expense/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/expense/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/expense/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/expense/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/expense/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/expense/delete/{id}`

<h3 id="expenseadminapi_deleteexpense-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_deleteexpense-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_deleteexpense-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_CreateExpense

<a id="opIdExpenseAdminApi_CreateExpense"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/expense/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/expense/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "ExpenseId": 0,
  "BranchId": 0,
  "BranchName": "string",
  "CategoryId": 0,
  "CategoryName": "string",
  "UserId": 0,
  "UserName": "string",
  "Amount": 1000,
  "ExpenseDate": "2019-08-24T14:15:22Z",
  "Note": "string",
  "RefImage": "string",
  "PaymentMethod": "string",
  "Status": "string",
  "ApprovedBy": 0,
  "ApproverName": "string",
  "ApprovedAt": "2019-08-24T14:15:22Z",
  "RejectedBy": 0,
  "RejectorName": "string",
  "RejectedAt": "2019-08-24T14:15:22Z",
  "RejectionReason": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/expense/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/expense/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/expense/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/expense/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/expense/create`

> Body parameter

```json
{
  "ExpenseId": 0,
  "BranchId": 0,
  "BranchName": "string",
  "CategoryId": 0,
  "CategoryName": "string",
  "UserId": 0,
  "UserName": "string",
  "Amount": 1000,
  "ExpenseDate": "2019-08-24T14:15:22Z",
  "Note": "string",
  "RefImage": "string",
  "PaymentMethod": "string",
  "Status": "string",
  "ApprovedBy": 0,
  "ApproverName": "string",
  "ApprovedAt": "2019-08-24T14:15:22Z",
  "RejectedBy": 0,
  "RejectorName": "string",
  "RejectedAt": "2019-08-24T14:15:22Z",
  "RejectionReason": "string"
}
```

```
ExpenseId: 0
BranchId: 0
BranchName: string
CategoryId: 0
CategoryName: string
UserId: 0
UserName: string
Amount: 1000
ExpenseDate: 2019-08-24T14:15:22Z
Note: string
RefImage: string
PaymentMethod: string
Status: string
ApprovedBy: 0
ApproverName: string
ApprovedAt: 2019-08-24T14:15:22Z
RejectedBy: 0
RejectorName: string
RejectedAt: 2019-08-24T14:15:22Z
RejectionReason: string

```

```yaml
ExpenseId: 0
BranchId: 0
BranchName: string
CategoryId: 0
CategoryName: string
UserId: 0
UserName: string
Amount: 1000
ExpenseDate: 2019-08-24T14:15:22Z
Note: string
RefImage: string
PaymentMethod: string
Status: string
ApprovedBy: 0
ApproverName: string
ApprovedAt: 2019-08-24T14:15:22Z
RejectedBy: 0
RejectorName: string
RejectedAt: 2019-08-24T14:15:22Z
RejectionReason: string

```

<h3 id="expenseadminapi_createexpense-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ExpenseDTO](#schemaexpensedto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_createexpense-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_createexpense-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_UpdateExpenseStatus

<a id="opIdExpenseAdminApi_UpdateExpenseStatus"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/expense/{id}/status \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/expense/{id}/status HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "ExpenseId": 0,
  "Status": "string",
  "PaymentMethod": "string",
  "RejectReason": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/{id}/status',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/expense/{id}/status',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/expense/{id}/status', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/expense/{id}/status', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/{id}/status");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/expense/{id}/status", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/expense/{id}/status`

> Body parameter

```json
{
  "ExpenseId": 0,
  "Status": "string",
  "PaymentMethod": "string",
  "RejectReason": "string"
}
```

```
ExpenseId: 0
Status: string
PaymentMethod: string
RejectReason: string

```

```yaml
ExpenseId: 0
Status: string
PaymentMethod: string
RejectReason: string

```

<h3 id="expenseadminapi_updateexpensestatus-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[UpdateExpenseStatusRequest](#schemaupdateexpensestatusrequest)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_updateexpensestatus-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_updateexpensestatus-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_GetAllExpenseCategories

<a id="opIdExpenseAdminApi_GetAllExpenseCategories"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/expense/categories \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/expense/categories HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/categories',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/expense/categories',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/expense/categories', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/expense/categories', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/categories");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/expense/categories", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/expense/categories`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_getallexpensecategories-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_getallexpensecategories-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_GetActiveExpenseCategories

<a id="opIdExpenseAdminApi_GetActiveExpenseCategories"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/expense/categories/active \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/expense/categories/active HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/categories/active',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/expense/categories/active',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/expense/categories/active', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/expense/categories/active', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/categories/active");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/expense/categories/active", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/expense/categories/active`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_getactiveexpensecategories-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_getactiveexpensecategories-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_GetExpenseCategoryById

<a id="opIdExpenseAdminApi_GetExpenseCategoryById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/expense/categories/{categoryId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/expense/categories/{categoryId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/categories/{categoryId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/expense/categories/{categoryId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/expense/categories/{categoryId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/expense/categories/{categoryId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/categories/{categoryId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/expense/categories/{categoryId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/expense/categories/{categoryId}`

<h3 id="expenseadminapi_getexpensecategorybyid-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|categoryId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_getexpensecategorybyid-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_getexpensecategorybyid-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_DeleteExpenseCategory

<a id="opIdExpenseAdminApi_DeleteExpenseCategory"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/expense/categories/delete/{categoryId} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/expense/categories/delete/{categoryId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/categories/delete/{categoryId}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/expense/categories/delete/{categoryId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/expense/categories/delete/{categoryId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/expense/categories/delete/{categoryId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/categories/delete/{categoryId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/expense/categories/delete/{categoryId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/expense/categories/delete/{categoryId}`

<h3 id="expenseadminapi_deleteexpensecategory-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|categoryId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_deleteexpensecategory-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_deleteexpensecategory-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_CreateExpenseCategory

<a id="opIdExpenseAdminApi_CreateExpenseCategory"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/expense/categories/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/expense/categories/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "IsActive": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/categories/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/expense/categories/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/expense/categories/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/expense/categories/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/categories/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/expense/categories/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/expense/categories/create`

> Body parameter

```json
{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "IsActive": true
}
```

```
CategoryId: 0
Name: string
Description: string
IsActive: true

```

```yaml
CategoryId: 0
Name: string
Description: string
IsActive: true

```

<h3 id="expenseadminapi_createexpensecategory-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ExpenseCategoryDTO](#schemaexpensecategorydto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_createexpensecategory-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_createexpensecategory-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## ExpenseAdminApi_UpdateExpenseCategory

<a id="opIdExpenseAdminApi_UpdateExpenseCategory"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/expense/categories/{id} \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/expense/categories/{id} HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "IsActive": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/expense/categories/{id}',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/expense/categories/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/expense/categories/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/expense/categories/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/expense/categories/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/expense/categories/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/expense/categories/{id}`

> Body parameter

```json
{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "IsActive": true
}
```

```
CategoryId: 0
Name: string
Description: string
IsActive: true

```

```yaml
CategoryId: 0
Name: string
Description: string
IsActive: true

```

<h3 id="expenseadminapi_updateexpensecategory-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[ExpenseCategoryDTO](#schemaexpensecategorydto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="expenseadminapi_updateexpensecategory-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="expenseadminapi_updateexpensecategory-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-inventoryadminapi">InventoryAdminApi</h1>

## Lấy nhật ký biến động kho (Phân trang DataTable &amp; Mobile).

<a id="opIdInventoryAdminApi_GetStockMovements"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/stock/movements \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/stock/movements HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/stock/movements',
{
  method: 'POST',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/stock/movements',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/stock/movements', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/stock/movements', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/stock/movements");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/stock/movements", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/stock/movements`

<h3 id="lấy-nhật-ký-biến-động-kho-(phân-trang-datatable-&amp;-mobile).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|warehouseId|query|integer(int32)|false|none|
|type|query|string|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|
|keyword|query|string|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-nhật-ký-biến-động-kho-(phân-trang-datatable-&amp;-mobile).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-nhật-ký-biến-động-kho-(phân-trang-datatable-&amp;-mobile).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy tổng tồn kho thực tế của 1 sản phẩm tại 1 kho.

<a id="opIdInventoryAdminApi_GetTotalActualStock"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/stock/total-actual?productId=0&warehouseId=0 \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/stock/total-actual?productId=0&warehouseId=0 HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/stock/total-actual?productId=0&warehouseId=0',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/stock/total-actual',
  params: {
  'productId' => 'integer(int32)',
'warehouseId' => 'integer(int32)'
}, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/stock/total-actual', params={
  'productId': '0',  'warehouseId': '0'
}, headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/stock/total-actual', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/stock/total-actual?productId=0&warehouseId=0");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/stock/total-actual", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/stock/total-actual`

<h3 id="lấy-tổng-tồn-kho-thực-tế-của-1-sản-phẩm-tại-1-kho.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|productId|query|integer(int32)|true|none|
|warehouseId|query|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tổng-tồn-kho-thực-tế-của-1-sản-phẩm-tại-1-kho.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tổng-tồn-kho-thực-tế-của-1-sản-phẩm-tại-1-kho.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Thực hiện điều chỉnh kho.

<a id="opIdInventoryAdminApi_AdjustStock"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/stock/adjust \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/stock/adjust HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "MovementType": "string",
  "WarehouseId": 0,
  "ProductId": 0,
  "QuantityChange": 0.1,
  "Reason": "string",
  "UserId": 0
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/stock/adjust',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/stock/adjust',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/stock/adjust', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/stock/adjust', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/stock/adjust");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/stock/adjust", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/stock/adjust`

> Body parameter

```json
{
  "MovementType": "string",
  "WarehouseId": 0,
  "ProductId": 0,
  "QuantityChange": 0.1,
  "Reason": "string",
  "UserId": 0
}
```

```
MovementType: string
WarehouseId: 0
ProductId: 0
QuantityChange: 0.1
Reason: string
UserId: 0

```

```yaml
MovementType: string
WarehouseId: 0
ProductId: 0
QuantityChange: 0.1
Reason: string
UserId: 0

```

<h3 id="thực-hiện-điều-chỉnh-kho.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[StockAdjustmentDTO](#schemastockadjustmentdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="thực-hiện-điều-chỉnh-kho.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="thực-hiện-điều-chỉnh-kho.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách tồn kho theo bộ lọc (productId, warehouseId, branchId).

<a id="opIdInventoryAdminApi_GetInventoryItems"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/items \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/items HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/items',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/items',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/items', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/items', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/items");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/items", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/items`

<h3 id="lấy-danh-sách-tồn-kho-theo-bộ-lọc-(productid,-warehouseid,-branchid).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|productId|query|integer(int32)|false|none|
|warehouseId|query|integer(int32)|false|none|
|branchId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-tồn-kho-theo-bộ-lọc-(productid,-warehouseid,-branchid).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-tồn-kho-theo-bộ-lọc-(productid,-warehouseid,-branchid).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Cập nhật bản ghi tồn kho.

<a id="opIdInventoryAdminApi_UpdateInventory"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/items/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/items/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "InventoryId": 0,
  "LotId": 0,
  "WarehouseId": 0,
  "Quantity": 0,
  "LastUpdate": "2019-08-24T14:15:22Z",
  "BranchName": "string",
  "WarehouseName": "string",
  "BatchCode": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/items/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/items/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/items/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/items/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/items/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/items/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/items/update`

> Body parameter

```json
{
  "InventoryId": 0,
  "LotId": 0,
  "WarehouseId": 0,
  "Quantity": 0,
  "LastUpdate": "2019-08-24T14:15:22Z",
  "BranchName": "string",
  "WarehouseName": "string",
  "BatchCode": "string"
}
```

```
InventoryId: 0
LotId: 0
WarehouseId: 0
Quantity: 0
LastUpdate: 2019-08-24T14:15:22Z
BranchName: string
WarehouseName: string
BatchCode: string

```

```yaml
InventoryId: 0
LotId: 0
WarehouseId: 0
Quantity: 0
LastUpdate: 2019-08-24T14:15:22Z
BranchName: string
WarehouseName: string
BatchCode: string

```

<h3 id="cập-nhật-bản-ghi-tồn-kho.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[InventoryDTO](#schemainventorydto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="cập-nhật-bản-ghi-tồn-kho.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="cập-nhật-bản-ghi-tồn-kho.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tạo bản ghi tồn kho mới.

<a id="opIdInventoryAdminApi_CreateInventory"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/items \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/items HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "InventoryId": 0,
  "LotId": 0,
  "WarehouseId": 0,
  "Quantity": 0,
  "LastUpdate": "2019-08-24T14:15:22Z",
  "BranchName": "string",
  "WarehouseName": "string",
  "BatchCode": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/items',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/items',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/items', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/items', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/items");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/items", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/items`

> Body parameter

```json
{
  "InventoryId": 0,
  "LotId": 0,
  "WarehouseId": 0,
  "Quantity": 0,
  "LastUpdate": "2019-08-24T14:15:22Z",
  "BranchName": "string",
  "WarehouseName": "string",
  "BatchCode": "string"
}
```

```
InventoryId: 0
LotId: 0
WarehouseId: 0
Quantity: 0
LastUpdate: 2019-08-24T14:15:22Z
BranchName: string
WarehouseName: string
BatchCode: string

```

```yaml
InventoryId: 0
LotId: 0
WarehouseId: 0
Quantity: 0
LastUpdate: 2019-08-24T14:15:22Z
BranchName: string
WarehouseName: string
BatchCode: string

```

<h3 id="tạo-bản-ghi-tồn-kho-mới.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[InventoryDTO](#schemainventorydto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tạo-bản-ghi-tồn-kho-mới.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tạo-bản-ghi-tồn-kho-mới.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy chi tiết bản ghi tồn kho theo ID.

<a id="opIdInventoryAdminApi_GetInventoryById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/items/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/items/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/items/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/items/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/items/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/items/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/items/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/items/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/items/{id}`

<h3 id="lấy-chi-tiết-bản-ghi-tồn-kho-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-chi-tiết-bản-ghi-tồn-kho-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-chi-tiết-bản-ghi-tồn-kho-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Xóa bản ghi tồn kho.

<a id="opIdInventoryAdminApi_DeleteInventory"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/inventory/items/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/inventory/items/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/items/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/inventory/items/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/inventory/items/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/inventory/items/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/items/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/inventory/items/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/inventory/items/delete/{id}`

<h3 id="xóa-bản-ghi-tồn-kho.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="xóa-bản-ghi-tồn-kho.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="xóa-bản-ghi-tồn-kho.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy tất cả đơn nhập hàng.

<a id="opIdInventoryAdminApi_GetAllPurchases"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/purchase/all \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/purchase/all HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/purchase/all',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/purchase/all',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/purchase/all', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/purchase/all', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/purchase/all");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/purchase/all", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/purchase/all`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tất-cả-đơn-nhập-hàng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tất-cả-đơn-nhập-hàng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tìm kiếm đơn nhập hàng với phân trang và nhiều tiêu chí lọc.

<a id="opIdInventoryAdminApi_SearchPurchases"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/purchase/search \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/purchase/search HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/purchase/search',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/purchase/search',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/purchase/search', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/purchase/search', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/purchase/search");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/purchase/search", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/purchase/search`

<h3 id="tìm-kiếm-đơn-nhập-hàng-với-phân-trang-và-nhiều-tiêu-chí-lọc.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|keyword|query|string|false|none|
|supplierId|query|integer(int32)|false|none|
|branchId|query|integer(int32)|false|none|
|warehouseId|query|integer(int32)|false|none|
|status|query|string|false|none|
|paymentStatus|query|string|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tìm-kiếm-đơn-nhập-hàng-với-phân-trang-và-nhiều-tiêu-chí-lọc.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tìm-kiếm-đơn-nhập-hàng-với-phân-trang-và-nhiều-tiêu-chí-lọc.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy đơn nhập hàng theo ID.

<a id="opIdInventoryAdminApi_GetPurchaseById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/purchase/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/purchase/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/purchase/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/purchase/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/purchase/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/purchase/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/purchase/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/purchase/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/purchase/{id}`

<h3 id="lấy-đơn-nhập-hàng-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-đơn-nhập-hàng-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-đơn-nhập-hàng-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Xóa đơn nhập hàng.

<a id="opIdInventoryAdminApi_DeletePurchase"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/inventory/purchase/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/inventory/purchase/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/purchase/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/inventory/purchase/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/inventory/purchase/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/inventory/purchase/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/purchase/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/inventory/purchase/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/inventory/purchase/delete/{id}`

<h3 id="xóa-đơn-nhập-hàng.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="xóa-đơn-nhập-hàng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="xóa-đơn-nhập-hàng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Cập nhật đơn nhập hàng.

<a id="opIdInventoryAdminApi_UpdatePurchase"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/purchase/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/purchase/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "WarehouseId": 0,
  "UserId": 0,
  "BranchId": 0,
  "OrderDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "TotalAmount": 0.1,
  "PaymentStatus": "string",
  "Notes": "string",
  "Details": [
    {
      "PurchaseOrderDetailId": 0,
      "PurchaseOrderId": 0,
      "ProductId": 0,
      "CategoryId": 0,
      "LotId": 0,
      "Quantity": 0,
      "Unit": "string",
      "UnitPrice": 0.1,
      "TotalPrice": 0.1,
      "ProductName": "string",
      "CategoryName": "string",
      "ProductLots": [
        {
          "LotId": 0,
          "ProductId": 0,
          "ExpiryDate": "2019-08-24T14:15:22Z",
          "BatchCode": "string",
          "CostPrice": 0.1,
          "ManufacturingDate": "2019-08-24T14:15:22Z"
        }
      ]
    }
  ],
  "SupplierDetail": [
    {
      "DebtId": 0,
      "PurchaseOrderId": 0,
      "SupplierId": 0,
      "SupplierName": "string",
      "TotalAmount": 0.1,
      "PaidAmount": 0.1,
      "UnpaidAmount": 0.1,
      "DueDate": "2019-08-24T14:15:22Z",
      "Status": "string",
      "CreatedAt": "2019-08-24T14:15:22Z",
      "UpdatedAt": "2019-08-24T14:15:22Z",
      "SupplierEmail": "string"
    }
  ],
  "PaidAmount": 0.1,
  "DueAmount": 0.1,
  "SupplierName": "string",
  "WarehouseName": "string",
  "UserName": "string",
  "BranchName": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/purchase/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/purchase/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/purchase/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/purchase/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/purchase/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/purchase/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/purchase/update`

> Body parameter

```json
{
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "WarehouseId": 0,
  "UserId": 0,
  "BranchId": 0,
  "OrderDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "TotalAmount": 0.1,
  "PaymentStatus": "string",
  "Notes": "string",
  "Details": [
    {
      "PurchaseOrderDetailId": 0,
      "PurchaseOrderId": 0,
      "ProductId": 0,
      "CategoryId": 0,
      "LotId": 0,
      "Quantity": 0,
      "Unit": "string",
      "UnitPrice": 0.1,
      "TotalPrice": 0.1,
      "ProductName": "string",
      "CategoryName": "string",
      "ProductLots": [
        {
          "LotId": 0,
          "ProductId": 0,
          "ExpiryDate": "2019-08-24T14:15:22Z",
          "BatchCode": "string",
          "CostPrice": 0.1,
          "ManufacturingDate": "2019-08-24T14:15:22Z"
        }
      ]
    }
  ],
  "SupplierDetail": [
    {
      "DebtId": 0,
      "PurchaseOrderId": 0,
      "SupplierId": 0,
      "SupplierName": "string",
      "TotalAmount": 0.1,
      "PaidAmount": 0.1,
      "UnpaidAmount": 0.1,
      "DueDate": "2019-08-24T14:15:22Z",
      "Status": "string",
      "CreatedAt": "2019-08-24T14:15:22Z",
      "UpdatedAt": "2019-08-24T14:15:22Z",
      "SupplierEmail": "string"
    }
  ],
  "PaidAmount": 0.1,
  "DueAmount": 0.1,
  "SupplierName": "string",
  "WarehouseName": "string",
  "UserName": "string",
  "BranchName": "string"
}
```

```
PurchaseOrderId: 0
SupplierId: 0
WarehouseId: 0
UserId: 0
BranchId: 0
OrderDate: 2019-08-24T14:15:22Z
Status: string
TotalAmount: 0.1
PaymentStatus: string
Notes: string
Details:
  - PurchaseOrderDetailId: 0
    PurchaseOrderId: 0
    ProductId: 0
    CategoryId: 0
    LotId: 0
    Quantity: 0
    Unit: string
    UnitPrice: 0.1
    TotalPrice: 0.1
    ProductName: string
    CategoryName: string
    ProductLots:
      - LotId: 0
        ProductId: 0
        ExpiryDate: 2019-08-24T14:15:22Z
        BatchCode: string
        CostPrice: 0.1
        ManufacturingDate: 2019-08-24T14:15:22Z
SupplierDetail:
  - DebtId: 0
    PurchaseOrderId: 0
    SupplierId: 0
    SupplierName: string
    TotalAmount: 0.1
    PaidAmount: 0.1
    UnpaidAmount: 0.1
    DueDate: 2019-08-24T14:15:22Z
    Status: string
    CreatedAt: 2019-08-24T14:15:22Z
    UpdatedAt: 2019-08-24T14:15:22Z
    SupplierEmail: string
PaidAmount: 0.1
DueAmount: 0.1
SupplierName: string
WarehouseName: string
UserName: string
BranchName: string

```

```yaml
PurchaseOrderId: 0
SupplierId: 0
WarehouseId: 0
UserId: 0
BranchId: 0
OrderDate: 2019-08-24T14:15:22Z
Status: string
TotalAmount: 0.1
PaymentStatus: string
Notes: string
Details:
  - PurchaseOrderDetailId: 0
    PurchaseOrderId: 0
    ProductId: 0
    CategoryId: 0
    LotId: 0
    Quantity: 0
    Unit: string
    UnitPrice: 0.1
    TotalPrice: 0.1
    ProductName: string
    CategoryName: string
    ProductLots:
      - LotId: 0
        ProductId: 0
        ExpiryDate: 2019-08-24T14:15:22Z
        BatchCode: string
        CostPrice: 0.1
        ManufacturingDate: 2019-08-24T14:15:22Z
SupplierDetail:
  - DebtId: 0
    PurchaseOrderId: 0
    SupplierId: 0
    SupplierName: string
    TotalAmount: 0.1
    PaidAmount: 0.1
    UnpaidAmount: 0.1
    DueDate: 2019-08-24T14:15:22Z
    Status: string
    CreatedAt: 2019-08-24T14:15:22Z
    UpdatedAt: 2019-08-24T14:15:22Z
    SupplierEmail: string
PaidAmount: 0.1
DueAmount: 0.1
SupplierName: string
WarehouseName: string
UserName: string
BranchName: string

```

<h3 id="cập-nhật-đơn-nhập-hàng.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[PurchaseOrderDTO](#schemapurchaseorderdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="cập-nhật-đơn-nhập-hàng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="cập-nhật-đơn-nhập-hàng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tạo / Lưu đơn nhập hàng mới.

<a id="opIdInventoryAdminApi_CreatePurchase"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/purchase \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/purchase HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "WarehouseId": 0,
  "UserId": 0,
  "BranchId": 0,
  "OrderDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "TotalAmount": 0.1,
  "PaymentStatus": "string",
  "Notes": "string",
  "Details": [
    {
      "PurchaseOrderDetailId": 0,
      "PurchaseOrderId": 0,
      "ProductId": 0,
      "CategoryId": 0,
      "LotId": 0,
      "Quantity": 0,
      "Unit": "string",
      "UnitPrice": 0.1,
      "TotalPrice": 0.1,
      "ProductName": "string",
      "CategoryName": "string",
      "ProductLots": [
        {
          "LotId": 0,
          "ProductId": 0,
          "ExpiryDate": "2019-08-24T14:15:22Z",
          "BatchCode": "string",
          "CostPrice": 0.1,
          "ManufacturingDate": "2019-08-24T14:15:22Z"
        }
      ]
    }
  ],
  "SupplierDetail": [
    {
      "DebtId": 0,
      "PurchaseOrderId": 0,
      "SupplierId": 0,
      "SupplierName": "string",
      "TotalAmount": 0.1,
      "PaidAmount": 0.1,
      "UnpaidAmount": 0.1,
      "DueDate": "2019-08-24T14:15:22Z",
      "Status": "string",
      "CreatedAt": "2019-08-24T14:15:22Z",
      "UpdatedAt": "2019-08-24T14:15:22Z",
      "SupplierEmail": "string"
    }
  ],
  "PaidAmount": 0.1,
  "DueAmount": 0.1,
  "SupplierName": "string",
  "WarehouseName": "string",
  "UserName": "string",
  "BranchName": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/purchase',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/purchase',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/purchase', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/purchase', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/purchase");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/purchase", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/purchase`

> Body parameter

```json
{
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "WarehouseId": 0,
  "UserId": 0,
  "BranchId": 0,
  "OrderDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "TotalAmount": 0.1,
  "PaymentStatus": "string",
  "Notes": "string",
  "Details": [
    {
      "PurchaseOrderDetailId": 0,
      "PurchaseOrderId": 0,
      "ProductId": 0,
      "CategoryId": 0,
      "LotId": 0,
      "Quantity": 0,
      "Unit": "string",
      "UnitPrice": 0.1,
      "TotalPrice": 0.1,
      "ProductName": "string",
      "CategoryName": "string",
      "ProductLots": [
        {
          "LotId": 0,
          "ProductId": 0,
          "ExpiryDate": "2019-08-24T14:15:22Z",
          "BatchCode": "string",
          "CostPrice": 0.1,
          "ManufacturingDate": "2019-08-24T14:15:22Z"
        }
      ]
    }
  ],
  "SupplierDetail": [
    {
      "DebtId": 0,
      "PurchaseOrderId": 0,
      "SupplierId": 0,
      "SupplierName": "string",
      "TotalAmount": 0.1,
      "PaidAmount": 0.1,
      "UnpaidAmount": 0.1,
      "DueDate": "2019-08-24T14:15:22Z",
      "Status": "string",
      "CreatedAt": "2019-08-24T14:15:22Z",
      "UpdatedAt": "2019-08-24T14:15:22Z",
      "SupplierEmail": "string"
    }
  ],
  "PaidAmount": 0.1,
  "DueAmount": 0.1,
  "SupplierName": "string",
  "WarehouseName": "string",
  "UserName": "string",
  "BranchName": "string"
}
```

```
PurchaseOrderId: 0
SupplierId: 0
WarehouseId: 0
UserId: 0
BranchId: 0
OrderDate: 2019-08-24T14:15:22Z
Status: string
TotalAmount: 0.1
PaymentStatus: string
Notes: string
Details:
  - PurchaseOrderDetailId: 0
    PurchaseOrderId: 0
    ProductId: 0
    CategoryId: 0
    LotId: 0
    Quantity: 0
    Unit: string
    UnitPrice: 0.1
    TotalPrice: 0.1
    ProductName: string
    CategoryName: string
    ProductLots:
      - LotId: 0
        ProductId: 0
        ExpiryDate: 2019-08-24T14:15:22Z
        BatchCode: string
        CostPrice: 0.1
        ManufacturingDate: 2019-08-24T14:15:22Z
SupplierDetail:
  - DebtId: 0
    PurchaseOrderId: 0
    SupplierId: 0
    SupplierName: string
    TotalAmount: 0.1
    PaidAmount: 0.1
    UnpaidAmount: 0.1
    DueDate: 2019-08-24T14:15:22Z
    Status: string
    CreatedAt: 2019-08-24T14:15:22Z
    UpdatedAt: 2019-08-24T14:15:22Z
    SupplierEmail: string
PaidAmount: 0.1
DueAmount: 0.1
SupplierName: string
WarehouseName: string
UserName: string
BranchName: string

```

```yaml
PurchaseOrderId: 0
SupplierId: 0
WarehouseId: 0
UserId: 0
BranchId: 0
OrderDate: 2019-08-24T14:15:22Z
Status: string
TotalAmount: 0.1
PaymentStatus: string
Notes: string
Details:
  - PurchaseOrderDetailId: 0
    PurchaseOrderId: 0
    ProductId: 0
    CategoryId: 0
    LotId: 0
    Quantity: 0
    Unit: string
    UnitPrice: 0.1
    TotalPrice: 0.1
    ProductName: string
    CategoryName: string
    ProductLots:
      - LotId: 0
        ProductId: 0
        ExpiryDate: 2019-08-24T14:15:22Z
        BatchCode: string
        CostPrice: 0.1
        ManufacturingDate: 2019-08-24T14:15:22Z
SupplierDetail:
  - DebtId: 0
    PurchaseOrderId: 0
    SupplierId: 0
    SupplierName: string
    TotalAmount: 0.1
    PaidAmount: 0.1
    UnpaidAmount: 0.1
    DueDate: 2019-08-24T14:15:22Z
    Status: string
    CreatedAt: 2019-08-24T14:15:22Z
    UpdatedAt: 2019-08-24T14:15:22Z
    SupplierEmail: string
PaidAmount: 0.1
DueAmount: 0.1
SupplierName: string
WarehouseName: string
UserName: string
BranchName: string

```

<h3 id="tạo-/-lưu-đơn-nhập-hàng-mới.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[PurchaseOrderDTO](#schemapurchaseorderdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tạo-/-lưu-đơn-nhập-hàng-mới.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tạo-/-lưu-đơn-nhập-hàng-mới.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy tất cả công nợ nhà cung cấp.

<a id="opIdInventoryAdminApi_GetAllDebts"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/debt/all \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/debt/all HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/all',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/debt/all',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/debt/all', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/debt/all', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/all");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/debt/all", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/debt/all`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tất-cả-công-nợ-nhà-cung-cấp.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tất-cả-công-nợ-nhà-cung-cấp.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Thống kê tổng hợp công nợ nhà cung cấp (4 Thẻ Widget).

<a id="opIdInventoryAdminApi_GetDebtSummary"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/debt/summary \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/debt/summary HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/summary',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/debt/summary',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/debt/summary', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/debt/summary', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/summary");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/debt/summary", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/debt/summary`

<h3 id="thống-kê-tổng-hợp-công-nợ-nhà-cung-cấp-(4-thẻ-widget).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|supplierId|query|integer(int32)|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="thống-kê-tổng-hợp-công-nợ-nhà-cung-cấp-(4-thẻ-widget).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="thống-kê-tổng-hợp-công-nợ-nhà-cung-cấp-(4-thẻ-widget).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tìm kiếm công nợ nhà cung cấp với phân trang &amp; bộ lọc.

<a id="opIdInventoryAdminApi_GetFilteredDebts"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/debt/list \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/debt/list HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/list',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/debt/list',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/debt/list', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/debt/list', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/list");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/debt/list", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/debt/list`

<h3 id="tìm-kiếm-công-nợ-nhà-cung-cấp-với-phân-trang-&amp;-bộ-lọc.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|keyword|query|string|false|none|
|supplierId|query|integer(int32)|false|none|
|status|query|string|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tìm-kiếm-công-nợ-nhà-cung-cấp-với-phân-trang-&amp;-bộ-lọc.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tìm-kiếm-công-nợ-nhà-cung-cấp-với-phân-trang-&amp;-bộ-lọc.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy chi tiết công nợ theo ID.

<a id="opIdInventoryAdminApi_GetDebtById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/debt/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/debt/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/debt/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/debt/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/debt/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/debt/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/debt/{id}`

<h3 id="lấy-chi-tiết-công-nợ-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-chi-tiết-công-nợ-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-chi-tiết-công-nợ-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Cập nhật công nợ nhà cung cấp.

<a id="opIdInventoryAdminApi_UpdateSupplierDebt"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/debt/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/debt/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "DebtId": 0,
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "SupplierName": "string",
  "TotalAmount": 0.1,
  "PaidAmount": 0.1,
  "UnpaidAmount": 0.1,
  "DueDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "SupplierEmail": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/debt/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/debt/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/debt/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/debt/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/debt/update`

> Body parameter

```json
{
  "DebtId": 0,
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "SupplierName": "string",
  "TotalAmount": 0.1,
  "PaidAmount": 0.1,
  "UnpaidAmount": 0.1,
  "DueDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "SupplierEmail": "string"
}
```

```
DebtId: 0
PurchaseOrderId: 0
SupplierId: 0
SupplierName: string
TotalAmount: 0.1
PaidAmount: 0.1
UnpaidAmount: 0.1
DueDate: 2019-08-24T14:15:22Z
Status: string
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
SupplierEmail: string

```

```yaml
DebtId: 0
PurchaseOrderId: 0
SupplierId: 0
SupplierName: string
TotalAmount: 0.1
PaidAmount: 0.1
UnpaidAmount: 0.1
DueDate: 2019-08-24T14:15:22Z
Status: string
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
SupplierEmail: string

```

<h3 id="cập-nhật-công-nợ-nhà-cung-cấp.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|query|integer(int32)|false|none|
|body|body|[SupplierDebtDTO](#schemasupplierdebtdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="cập-nhật-công-nợ-nhà-cung-cấp.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="cập-nhật-công-nợ-nhà-cung-cấp.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy công nợ theo mã Đơn nhập hàng (PurchaseOrderId).

<a id="opIdInventoryAdminApi_GetSupplierDebtByPurchaseOrder"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/debt/by-purchase-order/{purchaseOrderId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/debt/by-purchase-order/{purchaseOrderId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}`

<h3 id="lấy-công-nợ-theo-mã-đơn-nhập-hàng-(purchaseorderid).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|purchaseOrderId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-công-nợ-theo-mã-đơn-nhập-hàng-(purchaseorderid).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-công-nợ-theo-mã-đơn-nhập-hàng-(purchaseorderid).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tạo mới công nợ nhà cung cấp.

<a id="opIdInventoryAdminApi_CreateSupplierDebt"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/debt \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/debt HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "DebtId": 0,
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "SupplierName": "string",
  "TotalAmount": 0.1,
  "PaidAmount": 0.1,
  "UnpaidAmount": 0.1,
  "DueDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "SupplierEmail": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/debt',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/debt', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/debt', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/debt", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/debt`

> Body parameter

```json
{
  "DebtId": 0,
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "SupplierName": "string",
  "TotalAmount": 0.1,
  "PaidAmount": 0.1,
  "UnpaidAmount": 0.1,
  "DueDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "SupplierEmail": "string"
}
```

```
DebtId: 0
PurchaseOrderId: 0
SupplierId: 0
SupplierName: string
TotalAmount: 0.1
PaidAmount: 0.1
UnpaidAmount: 0.1
DueDate: 2019-08-24T14:15:22Z
Status: string
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
SupplierEmail: string

```

```yaml
DebtId: 0
PurchaseOrderId: 0
SupplierId: 0
SupplierName: string
TotalAmount: 0.1
PaidAmount: 0.1
UnpaidAmount: 0.1
DueDate: 2019-08-24T14:15:22Z
Status: string
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
SupplierEmail: string

```

<h3 id="tạo-mới-công-nợ-nhà-cung-cấp.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[SupplierDebtDTO](#schemasupplierdebtdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tạo-mới-công-nợ-nhà-cung-cấp.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tạo-mới-công-nợ-nhà-cung-cấp.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách công nợ sắp đến hạn (trong số ngày truyền vào).

<a id="opIdInventoryAdminApi_GetDebtsNearDueDate"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/debt/near-due \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/debt/near-due HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/near-due',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/debt/near-due',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/debt/near-due', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/debt/near-due', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/near-due");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/debt/near-due", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/debt/near-due`

<h3 id="lấy-danh-sách-công-nợ-sắp-đến-hạn-(trong-số-ngày-truyền-vào).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|daysBefore|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-công-nợ-sắp-đến-hạn-(trong-số-ngày-truyền-vào).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-công-nợ-sắp-đến-hạn-(trong-số-ngày-truyền-vào).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách công nợ quá hạn.

<a id="opIdInventoryAdminApi_GetOverdueDebts"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/debt/overdue \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/debt/overdue HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/overdue',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/debt/overdue',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/debt/overdue', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/debt/overdue', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/overdue");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/debt/overdue", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/debt/overdue`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-công-nợ-quá-hạn.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-công-nợ-quá-hạn.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách thanh toán của một khoản nợ.

<a id="opIdInventoryAdminApi_GetPaymentsByDebtId"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/debt/{debtId}/payments \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/debt/{debtId}/payments HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/{debtId}/payments',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/debt/{debtId}/payments',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/debt/{debtId}/payments', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/debt/{debtId}/payments', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/{debtId}/payments");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/debt/{debtId}/payments", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/debt/{debtId}/payments`

<h3 id="lấy-danh-sách-thanh-toán-của-một-khoản-nợ.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|debtId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-thanh-toán-của-một-khoản-nợ.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-thanh-toán-của-một-khoản-nợ.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tạo thanh toán công nợ cho NCC.

<a id="opIdInventoryAdminApi_CreateSupplierPayment"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/debt/payment \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/debt/payment HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "PaymentId": 0,
  "UserId": 0,
  "DebtId": 0,
  "Amount": 0.1,
  "PaymentMethod": "string",
  "PaymentDate": "2019-08-24T14:15:22Z",
  "PaymentProof": "string",
  "SupplierName": "string",
  "SupplierEmail": "string",
  "UnpaidAmountAfterPayment": 0.1
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/payment',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/debt/payment',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/debt/payment', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/debt/payment', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/payment");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/debt/payment", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/debt/payment`

> Body parameter

```json
{
  "PaymentId": 0,
  "UserId": 0,
  "DebtId": 0,
  "Amount": 0.1,
  "PaymentMethod": "string",
  "PaymentDate": "2019-08-24T14:15:22Z",
  "PaymentProof": "string",
  "SupplierName": "string",
  "SupplierEmail": "string",
  "UnpaidAmountAfterPayment": 0.1
}
```

```
PaymentId: 0
UserId: 0
DebtId: 0
Amount: 0.1
PaymentMethod: string
PaymentDate: 2019-08-24T14:15:22Z
PaymentProof: string
SupplierName: string
SupplierEmail: string
UnpaidAmountAfterPayment: 0.1

```

```yaml
PaymentId: 0
UserId: 0
DebtId: 0
Amount: 0.1
PaymentMethod: string
PaymentDate: 2019-08-24T14:15:22Z
PaymentProof: string
SupplierName: string
SupplierEmail: string
UnpaidAmountAfterPayment: 0.1

```

<h3 id="tạo-thanh-toán-công-nợ-cho-ncc.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[SupplierPaymentDTO](#schemasupplierpaymentdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tạo-thanh-toán-công-nợ-cho-ncc.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tạo-thanh-toán-công-nợ-cho-ncc.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Xóa phiếu thanh toán công nợ.

<a id="opIdInventoryAdminApi_DeleteSupplierPayment"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/inventory/debt/payment/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/inventory/debt/payment/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/debt/payment/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/inventory/debt/payment/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/inventory/debt/payment/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/inventory/debt/payment/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/debt/payment/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/inventory/debt/payment/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/inventory/debt/payment/delete/{id}`

<h3 id="xóa-phiếu-thanh-toán-công-nợ.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="xóa-phiếu-thanh-toán-công-nợ.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="xóa-phiếu-thanh-toán-công-nợ.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy tất cả kho hàng.

<a id="opIdInventoryAdminApi_GetAllWarehouses"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/warehouses/all \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/warehouses/all HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/warehouses/all',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/warehouses/all',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/warehouses/all', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/warehouses/all', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/warehouses/all");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/warehouses/all", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/warehouses/all`

<h3 id="lấy-tất-cả-kho-hàng.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|name|query|string|false|none|
|branchId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tất-cả-kho-hàng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tất-cả-kho-hàng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách kho theo Branch ID.

<a id="opIdInventoryAdminApi_GetWarehousesByBranch"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/warehouses/by-branch/{branchId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/warehouses/by-branch/{branchId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/warehouses/by-branch/{branchId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/warehouses/by-branch/{branchId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/warehouses/by-branch/{branchId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/warehouses/by-branch/{branchId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/warehouses/by-branch/{branchId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/warehouses/by-branch/{branchId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/warehouses/by-branch/{branchId}`

<h3 id="lấy-danh-sách-kho-theo-branch-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-kho-theo-branch-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-kho-theo-branch-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy chi tiết kho hàng theo ID.

<a id="opIdInventoryAdminApi_GetWarehouseById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/inventory/warehouses/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/inventory/warehouses/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/warehouses/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/inventory/warehouses/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/inventory/warehouses/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/inventory/warehouses/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/warehouses/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/inventory/warehouses/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/inventory/warehouses/{id}`

<h3 id="lấy-chi-tiết-kho-hàng-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-chi-tiết-kho-hàng-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-chi-tiết-kho-hàng-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Xóa kho hàng.

<a id="opIdInventoryAdminApi_DeleteWarehouse"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/inventory/warehouses/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/inventory/warehouses/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/warehouses/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/inventory/warehouses/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/inventory/warehouses/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/inventory/warehouses/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/warehouses/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/inventory/warehouses/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/inventory/warehouses/delete/{id}`

<h3 id="xóa-kho-hàng.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="xóa-kho-hàng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="xóa-kho-hàng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Cập nhật kho hàng.

<a id="opIdInventoryAdminApi_UpdateWarehouse"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/warehouses/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/warehouses/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "WarehouseId": 0,
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "BranchName": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/warehouses/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/warehouses/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/warehouses/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/warehouses/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/warehouses/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/warehouses/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/warehouses/update`

> Body parameter

```json
{
  "WarehouseId": 0,
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "BranchName": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true
}
```

```
WarehouseId: 0
BranchId: 0
Name: string
Address: string
AddressUrl: string
Latitude: 0.1
Longitude: 0.1
BranchName: string
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
CanBeDeleted: true

```

```yaml
WarehouseId: 0
BranchId: 0
Name: string
Address: string
AddressUrl: string
Latitude: 0.1
Longitude: 0.1
BranchName: string
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
CanBeDeleted: true

```

<h3 id="cập-nhật-kho-hàng.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[WarehouseDTO](#schemawarehousedto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="cập-nhật-kho-hàng.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="cập-nhật-kho-hàng.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tạo kho hàng mới.

<a id="opIdInventoryAdminApi_CreateWarehouse"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/inventory/warehouses \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/inventory/warehouses HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "WarehouseId": 0,
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "BranchName": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/inventory/warehouses',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/inventory/warehouses',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/inventory/warehouses', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/inventory/warehouses', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/inventory/warehouses");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/inventory/warehouses", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/inventory/warehouses`

> Body parameter

```json
{
  "WarehouseId": 0,
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "BranchName": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true
}
```

```
WarehouseId: 0
BranchId: 0
Name: string
Address: string
AddressUrl: string
Latitude: 0.1
Longitude: 0.1
BranchName: string
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
CanBeDeleted: true

```

```yaml
WarehouseId: 0
BranchId: 0
Name: string
Address: string
AddressUrl: string
Latitude: 0.1
Longitude: 0.1
BranchName: string
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
CanBeDeleted: true

```

<h3 id="tạo-kho-hàng-mới.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[WarehouseDTO](#schemawarehousedto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tạo-kho-hàng-mới.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tạo-kho-hàng-mới.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-logadminapi">LogAdminApi</h1>

## Truy vấn Audit Log (Ai làm gì, khi nào) với phân trang và bộ lọc.
GET: dùng query string.
POST: nhận AuditLogFilter từ request body.

<a id="opIdLogAdminApi_GetAuditLogs"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/logs/audit \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/logs/audit HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/logs/audit',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/logs/audit',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/logs/audit', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/logs/audit', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/logs/audit");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/logs/audit", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/logs/audit`

<h3 id="truy-vấn-audit-log-(ai-làm-gì,-khi-nào)-với-phân-trang-và-bộ-lọc.
get:-dùng-query-string.
post:-nhận-auditlogfilter-từ-request-body.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|tableName|query|string|false|none|
|action|query|string|false|none|
|search|query|string|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="truy-vấn-audit-log-(ai-làm-gì,-khi-nào)-với-phân-trang-và-bộ-lọc.
get:-dùng-query-string.
post:-nhận-auditlogfilter-từ-request-body.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="truy-vấn-audit-log-(ai-làm-gì,-khi-nào)-với-phân-trang-và-bộ-lọc.
get:-dùng-query-string.
post:-nhận-auditlogfilter-từ-request-body.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Truy vấn Audit Log bằng POST.
Dùng cho Android/Retrofit hoặc client gửi JSON body.

<a id="opIdLogAdminApi_PostAuditLogs"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/logs/audit \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/logs/audit HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "Start": 0,
  "Length": 0,
  "TableName": "string",
  "Action": "string",
  "Search": "string",
  "FromDate": "2019-08-24T14:15:22Z",
  "ToDate": "2019-08-24T14:15:22Z"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/logs/audit',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/logs/audit',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/logs/audit', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/logs/audit', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/logs/audit");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/logs/audit", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/logs/audit`

> Body parameter

```json
{
  "Start": 0,
  "Length": 0,
  "TableName": "string",
  "Action": "string",
  "Search": "string",
  "FromDate": "2019-08-24T14:15:22Z",
  "ToDate": "2019-08-24T14:15:22Z"
}
```

```
Start: 0
Length: 0
TableName: string
Action: string
Search: string
FromDate: 2019-08-24T14:15:22Z
ToDate: 2019-08-24T14:15:22Z

```

```yaml
Start: 0
Length: 0
TableName: string
Action: string
Search: string
FromDate: 2019-08-24T14:15:22Z
ToDate: 2019-08-24T14:15:22Z

```

<h3 id="truy-vấn-audit-log-bằng-post.
dùng-cho-android/retrofit-hoặc-client-gửi-json-body.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[AuditLogFilter](#schemaauditlogfilter)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="truy-vấn-audit-log-bằng-post.
dùng-cho-android/retrofit-hoặc-client-gửi-json-body.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="truy-vấn-audit-log-bằng-post.
dùng-cho-android/retrofit-hoặc-client-gửi-json-body.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy chi tiết một bản ghi Audit Log theo ID.

<a id="opIdLogAdminApi_GetAuditLogDetail"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/logs/audit/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/logs/audit/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/logs/audit/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/logs/audit/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/logs/audit/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/logs/audit/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/logs/audit/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/logs/audit/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/logs/audit/{id}`

<h3 id="lấy-chi-tiết-một-bản-ghi-audit-log-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int64)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-chi-tiết-một-bản-ghi-audit-log-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-chi-tiết-một-bản-ghi-audit-log-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Truy vấn Application Log (Error, Warning, Info) với phân trang và bộ lọc.
GET: dùng query string.
POST: nhận AppLogFilter từ request body.

<a id="opIdLogAdminApi_GetAppLogs"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/logs/app \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/logs/app HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/logs/app',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/logs/app',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/logs/app', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/logs/app', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/logs/app");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/logs/app", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/logs/app`

<h3 id="truy-vấn-application-log-(error,-warning,-info)-với-phân-trang-và-bộ-lọc.
get:-dùng-query-string.
post:-nhận-applogfilter-từ-request-body.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|logLevel|query|string|false|none|
|logger|query|string|false|none|
|search|query|string|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="truy-vấn-application-log-(error,-warning,-info)-với-phân-trang-và-bộ-lọc.
get:-dùng-query-string.
post:-nhận-applogfilter-từ-request-body.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="truy-vấn-application-log-(error,-warning,-info)-với-phân-trang-và-bộ-lọc.
get:-dùng-query-string.
post:-nhận-applogfilter-từ-request-body.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Truy vấn Application Log bằng POST.
Dùng cho Android/Retrofit hoặc client gửi JSON body.

<a id="opIdLogAdminApi_PostAppLogs"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/logs/app \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/logs/app HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "Start": 0,
  "Length": 0,
  "LogLevel": "string",
  "Logger": "string",
  "Search": "string",
  "FromDate": "2019-08-24T14:15:22Z",
  "ToDate": "2019-08-24T14:15:22Z"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/logs/app',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/logs/app',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/logs/app', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/logs/app', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/logs/app");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/logs/app", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/logs/app`

> Body parameter

```json
{
  "Start": 0,
  "Length": 0,
  "LogLevel": "string",
  "Logger": "string",
  "Search": "string",
  "FromDate": "2019-08-24T14:15:22Z",
  "ToDate": "2019-08-24T14:15:22Z"
}
```

```
Start: 0
Length: 0
LogLevel: string
Logger: string
Search: string
FromDate: 2019-08-24T14:15:22Z
ToDate: 2019-08-24T14:15:22Z

```

```yaml
Start: 0
Length: 0
LogLevel: string
Logger: string
Search: string
FromDate: 2019-08-24T14:15:22Z
ToDate: 2019-08-24T14:15:22Z

```

<h3 id="truy-vấn-application-log-bằng-post.
dùng-cho-android/retrofit-hoặc-client-gửi-json-body.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[AppLogFilter](#schemaapplogfilter)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="truy-vấn-application-log-bằng-post.
dùng-cho-android/retrofit-hoặc-client-gửi-json-body.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="truy-vấn-application-log-bằng-post.
dùng-cho-android/retrofit-hoặc-client-gửi-json-body.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy chi tiết một bản ghi Application Log theo ID.

<a id="opIdLogAdminApi_GetAppLogDetail"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/logs/app/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/logs/app/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/logs/app/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/logs/app/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/logs/app/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/logs/app/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/logs/app/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/logs/app/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/logs/app/{id}`

<h3 id="lấy-chi-tiết-một-bản-ghi-application-log-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int64)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-chi-tiết-một-bản-ghi-application-log-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-chi-tiết-một-bản-ghi-application-log-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy thống kê tổng hợp log (Tổng số, Error count, Warning count...).

<a id="opIdLogAdminApi_GetLogStatistics"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/logs/stats \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/logs/stats HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/logs/stats',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/logs/stats',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/logs/stats', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/logs/stats', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/logs/stats");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/logs/stats", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/logs/stats`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-thống-kê-tổng-hợp-log-(tổng-số,-error-count,-warning-count...).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-thống-kê-tổng-hợp-log-(tổng-số,-error-count,-warning-count...).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách sự kiện hệ thống mới nhất (System Events).

<a id="opIdLogAdminApi_GetLatestSystemEvents"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/logs/system-events \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/logs/system-events HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/logs/system-events',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/logs/system-events',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/logs/system-events', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/logs/system-events', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/logs/system-events");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/logs/system-events", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/logs/system-events`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-sự-kiện-hệ-thống-mới-nhất-(system-events).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-sự-kiện-hệ-thống-mới-nhất-(system-events).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-productadminapi">ProductAdminApi</h1>

## Lấy tất cả sản phẩm đang hoạt động.

<a id="opIdProductAdminApi_GetAllProducts"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/products \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/products HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/products',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/products', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/products', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/products", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/products`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tất-cả-sản-phẩm-đang-hoạt-động.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tất-cả-sản-phẩm-đang-hoạt-động.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Cập nhật sản phẩm.

<a id="opIdProductAdminApi_UpdateProduct"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/products/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/products/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "ProductId": 0,
  "Name": "string",
  "CategoryId": 0,
  "SupplierId": 0,
  "Barcode": "string",
  "Price": 0.1,
  "Unit": "string",
  "Description": "string",
  "MinStock": 0,
  "MaxStock": 0,
  "Image": "string",
  "CategoryName": "string",
  "SupplierName": "string",
  "Quantity": 0,
  "ExpiredAt": "2019-08-24T14:15:22Z",
  "OriginalPrice": 0.1,
  "FinalPrice": 0.1,
  "PromotionName": "string",
  "DiscountAmount": 0.1
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/products/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/products/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/products/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/products/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/products/update`

> Body parameter

```json
{
  "ProductId": 0,
  "Name": "string",
  "CategoryId": 0,
  "SupplierId": 0,
  "Barcode": "string",
  "Price": 0.1,
  "Unit": "string",
  "Description": "string",
  "MinStock": 0,
  "MaxStock": 0,
  "Image": "string",
  "CategoryName": "string",
  "SupplierName": "string",
  "Quantity": 0,
  "ExpiredAt": "2019-08-24T14:15:22Z",
  "OriginalPrice": 0.1,
  "FinalPrice": 0.1,
  "PromotionName": "string",
  "DiscountAmount": 0.1
}
```

```
ProductId: 0
Name: string
CategoryId: 0
SupplierId: 0
Barcode: string
Price: 0.1
Unit: string
Description: string
MinStock: 0
MaxStock: 0
Image: string
CategoryName: string
SupplierName: string
Quantity: 0
ExpiredAt: 2019-08-24T14:15:22Z
OriginalPrice: 0.1
FinalPrice: 0.1
PromotionName: string
DiscountAmount: 0.1

```

```yaml
ProductId: 0
Name: string
CategoryId: 0
SupplierId: 0
Barcode: string
Price: 0.1
Unit: string
Description: string
MinStock: 0
MaxStock: 0
Image: string
CategoryName: string
SupplierName: string
Quantity: 0
ExpiredAt: 2019-08-24T14:15:22Z
OriginalPrice: 0.1
FinalPrice: 0.1
PromotionName: string
DiscountAmount: 0.1

```

<h3 id="cập-nhật-sản-phẩm.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ProductDTO](#schemaproductdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="cập-nhật-sản-phẩm.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="cập-nhật-sản-phẩm.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tạo mới sản phẩm.

<a id="opIdProductAdminApi_CreateProduct"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/products \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/products HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "ProductId": 0,
  "Name": "string",
  "CategoryId": 0,
  "SupplierId": 0,
  "Barcode": "string",
  "Price": 0.1,
  "Unit": "string",
  "Description": "string",
  "MinStock": 0,
  "MaxStock": 0,
  "Image": "string",
  "CategoryName": "string",
  "SupplierName": "string",
  "Quantity": 0,
  "ExpiredAt": "2019-08-24T14:15:22Z",
  "OriginalPrice": 0.1,
  "FinalPrice": 0.1,
  "PromotionName": "string",
  "DiscountAmount": 0.1
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/products',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/products', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/products', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/products", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/products`

> Body parameter

```json
{
  "ProductId": 0,
  "Name": "string",
  "CategoryId": 0,
  "SupplierId": 0,
  "Barcode": "string",
  "Price": 0.1,
  "Unit": "string",
  "Description": "string",
  "MinStock": 0,
  "MaxStock": 0,
  "Image": "string",
  "CategoryName": "string",
  "SupplierName": "string",
  "Quantity": 0,
  "ExpiredAt": "2019-08-24T14:15:22Z",
  "OriginalPrice": 0.1,
  "FinalPrice": 0.1,
  "PromotionName": "string",
  "DiscountAmount": 0.1
}
```

```
ProductId: 0
Name: string
CategoryId: 0
SupplierId: 0
Barcode: string
Price: 0.1
Unit: string
Description: string
MinStock: 0
MaxStock: 0
Image: string
CategoryName: string
SupplierName: string
Quantity: 0
ExpiredAt: 2019-08-24T14:15:22Z
OriginalPrice: 0.1
FinalPrice: 0.1
PromotionName: string
DiscountAmount: 0.1

```

```yaml
ProductId: 0
Name: string
CategoryId: 0
SupplierId: 0
Barcode: string
Price: 0.1
Unit: string
Description: string
MinStock: 0
MaxStock: 0
Image: string
CategoryName: string
SupplierName: string
Quantity: 0
ExpiredAt: 2019-08-24T14:15:22Z
OriginalPrice: 0.1
FinalPrice: 0.1
PromotionName: string
DiscountAmount: 0.1

```

<h3 id="tạo-mới-sản-phẩm.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ProductDTO](#schemaproductdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tạo-mới-sản-phẩm.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tạo-mới-sản-phẩm.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách sản phẩm có phân trang (dành cho Mobile &amp; Web DataTables).

<a id="opIdProductAdminApi_GetProductsPaged"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/products/paged \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/products/paged HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/paged',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/products/paged',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/products/paged', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/products/paged', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/paged");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/products/paged", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/products/paged`

<h3 id="lấy-danh-sách-sản-phẩm-có-phân-trang-(dành-cho-mobile-&amp;-web-datatables).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|keyword|query|string|false|none|
|categoryId|query|integer(int32)|false|none|
|branchId|query|integer(int32)|false|none|
|supplierId|query|integer(int32)|false|none|
|warehouseId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-sản-phẩm-có-phân-trang-(dành-cho-mobile-&amp;-web-datatables).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-sản-phẩm-có-phân-trang-(dành-cho-mobile-&amp;-web-datatables).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tìm kiếm nâng cao sản phẩm theo từ khóa, danh mục, chi nhánh, nhà cung cấp, kho.

<a id="opIdProductAdminApi_SearchProducts"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/products/search \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/products/search HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/search',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/products/search',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/products/search', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/products/search', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/search");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/products/search", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/products/search`

<h3 id="tìm-kiếm-nâng-cao-sản-phẩm-theo-từ-khóa,-danh-mục,-chi-nhánh,-nhà-cung-cấp,-kho.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|keyword|query|string|false|none|
|categoryId|query|integer(int32)|false|none|
|branchId|query|integer(int32)|false|none|
|supplierId|query|integer(int32)|false|none|
|warehouseId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tìm-kiếm-nâng-cao-sản-phẩm-theo-từ-khóa,-danh-mục,-chi-nhánh,-nhà-cung-cấp,-kho.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tìm-kiếm-nâng-cao-sản-phẩm-theo-từ-khóa,-danh-mục,-chi-nhánh,-nhà-cung-cấp,-kho.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tìm kiếm đơn giản sản phẩm theo từ khóa và chi nhánh.

<a id="opIdProductAdminApi_SearchProductsSimple"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/products/search-simple \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/products/search-simple HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/search-simple',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/products/search-simple',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/products/search-simple', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/products/search-simple', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/search-simple");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/products/search-simple", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/products/search-simple`

<h3 id="tìm-kiếm-đơn-giản-sản-phẩm-theo-từ-khóa-và-chi-nhánh.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|keyword|query|string|false|none|
|branchId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tìm-kiếm-đơn-giản-sản-phẩm-theo-từ-khóa-và-chi-nhánh.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tìm-kiếm-đơn-giản-sản-phẩm-theo-từ-khóa-và-chi-nhánh.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tìm kiếm sản phẩm bằng mã vạch (Barcode) quét từ camera điện thoại hoặc máy quét.

<a id="opIdProductAdminApi_SearchProductByBarcode"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/products/by-barcode?barcode=string \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/products/by-barcode?barcode=string HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/by-barcode?barcode=string',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/products/by-barcode',
  params: {
  'barcode' => 'string'
}, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/products/by-barcode', params={
  'barcode': 'string'
}, headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/products/by-barcode', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/by-barcode?barcode=string");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/products/by-barcode", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/products/by-barcode`

<h3 id="tìm-kiếm-sản-phẩm-bằng-mã-vạch-(barcode)-quét-từ-camera-điện-thoại-hoặc-máy-quét.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|barcode|query|string|true|none|
|branchId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tìm-kiếm-sản-phẩm-bằng-mã-vạch-(barcode)-quét-từ-camera-điện-thoại-hoặc-máy-quét.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tìm-kiếm-sản-phẩm-bằng-mã-vạch-(barcode)-quét-từ-camera-điện-thoại-hoặc-máy-quét.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy chi tiết sản phẩm theo ID.

<a id="opIdProductAdminApi_GetProductById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/products/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/products/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/products/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/products/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/products/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/products/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/products/{id}`

<h3 id="lấy-chi-tiết-sản-phẩm-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-chi-tiết-sản-phẩm-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-chi-tiết-sản-phẩm-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Xóa / Ẩn sản phẩm theo ID.

<a id="opIdProductAdminApi_DeleteProduct"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/product-management/products/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/product-management/products/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/product-management/products/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/product-management/products/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/product-management/products/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/product-management/products/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/product-management/products/delete/{id}`

<h3 id="xóa-/-ẩn-sản-phẩm-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="xóa-/-ẩn-sản-phẩm-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="xóa-/-ẩn-sản-phẩm-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách sản phẩm theo danh sách ID.

<a id="opIdProductAdminApi_GetProductsByIds"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/products/by-ids \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/products/by-ids HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '[
  0
]';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/by-ids',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/products/by-ids',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/products/by-ids', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/products/by-ids', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/by-ids");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/products/by-ids", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/products/by-ids`

> Body parameter

```json
[
  0
]
```

```
- 0

```

```yaml
- 0

```

<h3 id="lấy-danh-sách-sản-phẩm-theo-danh-sách-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|array[integer]|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-sản-phẩm-theo-danh-sách-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-sản-phẩm-theo-danh-sách-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy Bảng Tra Cứu Tên Sản Phẩm theo danh sách ID (Dictionary: ID -&gt; Name).

<a id="opIdProductAdminApi_GetProductNamesByIds"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/products/names-by-ids \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/products/names-by-ids HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '[
  0
]';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/names-by-ids',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/products/names-by-ids',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/products/names-by-ids', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/products/names-by-ids', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/names-by-ids");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/products/names-by-ids", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/products/names-by-ids`

> Body parameter

```json
[
  0
]
```

```
- 0

```

```yaml
- 0

```

<h3 id="lấy-bảng-tra-cứu-tên-sản-phẩm-theo-danh-sách-id-(dictionary:-id--&gt;-name).-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|array[integer]|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-bảng-tra-cứu-tên-sản-phẩm-theo-danh-sách-id-(dictionary:-id--&gt;-name).-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-bảng-tra-cứu-tên-sản-phẩm-theo-danh-sách-id-(dictionary:-id--&gt;-name).-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Kích hoạt lại sản phẩm bị ngưng hoạt động.

<a id="opIdProductAdminApi_ActiveProduct"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/products/active?productId=0 \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/products/active?productId=0 HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/active?productId=0',
{
  method: 'POST',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/products/active',
  params: {
  'productId' => 'integer(int32)'
}, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/products/active', params={
  'productId': '0'
}, headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/products/active', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/active?productId=0");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/products/active", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/products/active`

<h3 id="kích-hoạt-lại-sản-phẩm-bị-ngưng-hoạt-động.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|productId|query|integer(int32)|true|none|
|minStock|query|integer(int32)|false|none|
|maxStock|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="kích-hoạt-lại-sản-phẩm-bị-ngưng-hoạt-động.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="kích-hoạt-lại-sản-phẩm-bị-ngưng-hoạt-động.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách sản phẩm bị ngưng hoạt động.

<a id="opIdProductAdminApi_GetInactiveProducts"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/products/inactive \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/products/inactive HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/inactive',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/products/inactive',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/products/inactive', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/products/inactive', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/inactive");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/products/inactive", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/products/inactive`

<h3 id="lấy-danh-sách-sản-phẩm-bị-ngưng-hoạt-động.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|keyword|query|string|false|none|
|categoryId|query|integer(int32)|false|none|
|branchId|query|integer(int32)|false|none|
|supplierId|query|integer(int32)|false|none|
|warehouseId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-sản-phẩm-bị-ngưng-hoạt-động.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-sản-phẩm-bị-ngưng-hoạt-động.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách cảnh báo tồn kho thấp.

<a id="opIdProductAdminApi_GetLowStockAlerts"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/products/low-stock-alerts \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/products/low-stock-alerts HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/low-stock-alerts',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/products/low-stock-alerts',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/products/low-stock-alerts', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/products/low-stock-alerts', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/low-stock-alerts");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/products/low-stock-alerts", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/products/low-stock-alerts`

<h3 id="lấy-danh-sách-cảnh-báo-tồn-kho-thấp.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|top|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-cảnh-báo-tồn-kho-thấp.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-cảnh-báo-tồn-kho-thấp.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy tất cả ảnh phụ của một sản phẩm.

<a id="opIdProductAdminApi_GetProductImages"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/products/{productId}/images \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/products/{productId}/images HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/{productId}/images',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/products/{productId}/images',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/products/{productId}/images', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/products/{productId}/images', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/{productId}/images");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/products/{productId}/images", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/products/{productId}/images`

<h3 id="lấy-tất-cả-ảnh-phụ-của-một-sản-phẩm.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|productId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tất-cả-ảnh-phụ-của-một-sản-phẩm.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tất-cả-ảnh-phụ-của-một-sản-phẩm.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Thêm ảnh cho sản phẩm.

<a id="opIdProductAdminApi_CreateProductImage"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/products/images \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/products/images HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "ImageId": 0,
  "ProductId": 0,
  "ImageUrl": "string",
  "SortOrder": 0,
  "CreatedAt": "2019-08-24T14:15:22Z"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/images',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/products/images',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/products/images', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/products/images', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/images");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/products/images", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/products/images`

> Body parameter

```json
{
  "ImageId": 0,
  "ProductId": 0,
  "ImageUrl": "string",
  "SortOrder": 0,
  "CreatedAt": "2019-08-24T14:15:22Z"
}
```

```
ImageId: 0
ProductId: 0
ImageUrl: string
SortOrder: 0
CreatedAt: 2019-08-24T14:15:22Z

```

```yaml
ImageId: 0
ProductId: 0
ImageUrl: string
SortOrder: 0
CreatedAt: 2019-08-24T14:15:22Z

```

<h3 id="thêm-ảnh-cho-sản-phẩm.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ProductImageDTO](#schemaproductimagedto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="thêm-ảnh-cho-sản-phẩm.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="thêm-ảnh-cho-sản-phẩm.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Xóa ảnh phụ của sản phẩm.

<a id="opIdProductAdminApi_DeleteProductImage"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/product-management/products/images/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/product-management/products/images/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/products/images/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/product-management/products/images/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/product-management/products/images/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/product-management/products/images/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/products/images/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/product-management/products/images/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/product-management/products/images/delete/{id}`

<h3 id="xóa-ảnh-phụ-của-sản-phẩm.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="xóa-ảnh-phụ-của-sản-phẩm.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="xóa-ảnh-phụ-của-sản-phẩm.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy tất cả danh mục sản phẩm.

<a id="opIdProductAdminApi_GetAllCategories"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/categories \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/categories HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/categories',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/categories',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/categories', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/categories', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/categories");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/categories", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/categories`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tất-cả-danh-mục-sản-phẩm.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tất-cả-danh-mục-sản-phẩm.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Cập nhật danh mục.

<a id="opIdProductAdminApi_UpdateCategory"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/categories/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/categories/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "CanBeDeleted": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/categories/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/categories/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/categories/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/categories/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/categories/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/categories/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/categories/update`

> Body parameter

```json
{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "CanBeDeleted": true
}
```

```
CategoryId: 0
Name: string
Description: string
CanBeDeleted: true

```

```yaml
CategoryId: 0
Name: string
Description: string
CanBeDeleted: true

```

<h3 id="cập-nhật-danh-mục.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ProductCategoryDTO](#schemaproductcategorydto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="cập-nhật-danh-mục.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="cập-nhật-danh-mục.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tạo danh mục mới.

<a id="opIdProductAdminApi_CreateCategory"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/categories \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/categories HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "CanBeDeleted": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/categories',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/categories',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/categories', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/categories', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/categories");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/categories", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/categories`

> Body parameter

```json
{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "CanBeDeleted": true
}
```

```
CategoryId: 0
Name: string
Description: string
CanBeDeleted: true

```

```yaml
CategoryId: 0
Name: string
Description: string
CanBeDeleted: true

```

<h3 id="tạo-danh-mục-mới.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ProductCategoryDTO](#schemaproductcategorydto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tạo-danh-mục-mới.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tạo-danh-mục-mới.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy chi tiết danh mục theo ID.

<a id="opIdProductAdminApi_GetCategoryById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/categories/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/categories/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/categories/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/categories/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/categories/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/categories/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/categories/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/categories/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/categories/{id}`

<h3 id="lấy-chi-tiết-danh-mục-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-chi-tiết-danh-mục-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-chi-tiết-danh-mục-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Xóa danh mục.

<a id="opIdProductAdminApi_DeleteCategory"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/product-management/categories/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/product-management/categories/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/categories/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/product-management/categories/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/product-management/categories/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/product-management/categories/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/categories/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/product-management/categories/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/product-management/categories/delete/{id}`

<h3 id="xóa-danh-mục.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="xóa-danh-mục.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="xóa-danh-mục.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy tất cả nhà cung cấp.

<a id="opIdProductAdminApi_GetAllSuppliers"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/suppliers \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/suppliers HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/suppliers',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/suppliers',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/suppliers', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/suppliers', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/suppliers");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/suppliers", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/suppliers`

<h3 id="lấy-tất-cả-nhà-cung-cấp.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|name|query|string|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tất-cả-nhà-cung-cấp.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tất-cả-nhà-cung-cấp.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Cập nhật nhà cung cấp.

<a id="opIdProductAdminApi_UpdateSupplier"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/suppliers/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/suppliers/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "SupplierId": 0,
  "Name": "string",
  "Email": "string",
  "Phone": "string",
  "Address": "string",
  "AddressUrl": "string",
  "ContactPerson": "string",
  "CanBeDeleted": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/suppliers/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/suppliers/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/suppliers/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/suppliers/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/suppliers/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/suppliers/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/suppliers/update`

> Body parameter

```json
{
  "SupplierId": 0,
  "Name": "string",
  "Email": "string",
  "Phone": "string",
  "Address": "string",
  "AddressUrl": "string",
  "ContactPerson": "string",
  "CanBeDeleted": true
}
```

```
SupplierId: 0
Name: string
Email: string
Phone: string
Address: string
AddressUrl: string
ContactPerson: string
CanBeDeleted: true

```

```yaml
SupplierId: 0
Name: string
Email: string
Phone: string
Address: string
AddressUrl: string
ContactPerson: string
CanBeDeleted: true

```

<h3 id="cập-nhật-nhà-cung-cấp.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[SupplierDTO](#schemasupplierdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="cập-nhật-nhà-cung-cấp.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="cập-nhật-nhà-cung-cấp.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tạo nhà cung cấp mới.

<a id="opIdProductAdminApi_CreateSupplier"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/suppliers \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/suppliers HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "SupplierId": 0,
  "Name": "string",
  "Email": "string",
  "Phone": "string",
  "Address": "string",
  "AddressUrl": "string",
  "ContactPerson": "string",
  "CanBeDeleted": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/suppliers',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/suppliers',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/suppliers', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/suppliers', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/suppliers");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/suppliers", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/suppliers`

> Body parameter

```json
{
  "SupplierId": 0,
  "Name": "string",
  "Email": "string",
  "Phone": "string",
  "Address": "string",
  "AddressUrl": "string",
  "ContactPerson": "string",
  "CanBeDeleted": true
}
```

```
SupplierId: 0
Name: string
Email: string
Phone: string
Address: string
AddressUrl: string
ContactPerson: string
CanBeDeleted: true

```

```yaml
SupplierId: 0
Name: string
Email: string
Phone: string
Address: string
AddressUrl: string
ContactPerson: string
CanBeDeleted: true

```

<h3 id="tạo-nhà-cung-cấp-mới.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[SupplierDTO](#schemasupplierdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tạo-nhà-cung-cấp-mới.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tạo-nhà-cung-cấp-mới.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy thông tin nhà cung cấp theo ID.

<a id="opIdProductAdminApi_GetSupplierById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/suppliers/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/suppliers/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/suppliers/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/suppliers/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/suppliers/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/suppliers/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/suppliers/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/suppliers/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/suppliers/{id}`

<h3 id="lấy-thông-tin-nhà-cung-cấp-theo-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-thông-tin-nhà-cung-cấp-theo-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-thông-tin-nhà-cung-cấp-theo-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Xóa nhà cung cấp.

<a id="opIdProductAdminApi_DeleteSupplier"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/product-management/suppliers/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/product-management/suppliers/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/suppliers/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/product-management/suppliers/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/product-management/suppliers/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/product-management/suppliers/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/suppliers/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/product-management/suppliers/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/product-management/suppliers/delete/{id}`

<h3 id="xóa-nhà-cung-cấp.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="xóa-nhà-cung-cấp.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="xóa-nhà-cung-cấp.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy tất cả lô sản phẩm.

<a id="opIdProductAdminApi_GetAllLots"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/lots \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/lots HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/lots',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/lots',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/lots', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/lots', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/lots");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/lots", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/lots`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-tất-cả-lô-sản-phẩm.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-tất-cả-lô-sản-phẩm.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Cập nhật thông tin lô sản phẩm.

<a id="opIdProductAdminApi_UpdateLot"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/lots/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/lots/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "LotId": 0,
  "ProductId": 0,
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "BatchCode": "string",
  "CostPrice": 0.1,
  "ManufacturingDate": "2019-08-24T14:15:22Z"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/lots/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/lots/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/lots/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/lots/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/lots/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/lots/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/lots/update`

> Body parameter

```json
{
  "LotId": 0,
  "ProductId": 0,
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "BatchCode": "string",
  "CostPrice": 0.1,
  "ManufacturingDate": "2019-08-24T14:15:22Z"
}
```

```
LotId: 0
ProductId: 0
ExpiryDate: 2019-08-24T14:15:22Z
BatchCode: string
CostPrice: 0.1
ManufacturingDate: 2019-08-24T14:15:22Z

```

```yaml
LotId: 0
ProductId: 0
ExpiryDate: 2019-08-24T14:15:22Z
BatchCode: string
CostPrice: 0.1
ManufacturingDate: 2019-08-24T14:15:22Z

```

<h3 id="cập-nhật-thông-tin-lô-sản-phẩm.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ProductLotDTO](#schemaproductlotdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="cập-nhật-thông-tin-lô-sản-phẩm.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="cập-nhật-thông-tin-lô-sản-phẩm.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Tạo lô sản phẩm mới.

<a id="opIdProductAdminApi_CreateLot"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/product-management/lots \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/product-management/lots HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "LotId": 0,
  "ProductId": 0,
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "BatchCode": "string",
  "CostPrice": 0.1,
  "ManufacturingDate": "2019-08-24T14:15:22Z"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/lots',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/product-management/lots',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/product-management/lots', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/product-management/lots', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/lots");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/product-management/lots", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/product-management/lots`

> Body parameter

```json
{
  "LotId": 0,
  "ProductId": 0,
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "BatchCode": "string",
  "CostPrice": 0.1,
  "ManufacturingDate": "2019-08-24T14:15:22Z"
}
```

```
LotId: 0
ProductId: 0
ExpiryDate: 2019-08-24T14:15:22Z
BatchCode: string
CostPrice: 0.1
ManufacturingDate: 2019-08-24T14:15:22Z

```

```yaml
LotId: 0
ProductId: 0
ExpiryDate: 2019-08-24T14:15:22Z
BatchCode: string
CostPrice: 0.1
ManufacturingDate: 2019-08-24T14:15:22Z

```

<h3 id="tạo-lô-sản-phẩm-mới.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ProductLotDTO](#schemaproductlotdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="tạo-lô-sản-phẩm-mới.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="tạo-lô-sản-phẩm-mới.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy chi tiết lô theo Lot ID.

<a id="opIdProductAdminApi_GetLotById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/lots/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/lots/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/lots/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/lots/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/lots/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/lots/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/lots/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/lots/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/lots/{id}`

<h3 id="lấy-chi-tiết-lô-theo-lot-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-chi-tiết-lô-theo-lot-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-chi-tiết-lô-theo-lot-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Xóa lô sản phẩm.

<a id="opIdProductAdminApi_DeleteLot"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/product-management/lots/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/product-management/lots/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/lots/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/product-management/lots/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/product-management/lots/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/product-management/lots/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/lots/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/product-management/lots/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/product-management/lots/delete/{id}`

<h3 id="xóa-lô-sản-phẩm.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="xóa-lô-sản-phẩm.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="xóa-lô-sản-phẩm.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách lô theo Product ID.

<a id="opIdProductAdminApi_GetLotsByProductId"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/product-management/lots/by-product/{productId} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/product-management/lots/by-product/{productId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/product-management/lots/by-product/{productId}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/product-management/lots/by-product/{productId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/product-management/lots/by-product/{productId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/product-management/lots/by-product/{productId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/product-management/lots/by-product/{productId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/product-management/lots/by-product/{productId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/product-management/lots/by-product/{productId}`

<h3 id="lấy-danh-sách-lô-theo-product-id.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|productId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-lô-theo-product-id.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-lô-theo-product-id.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-quotationadminapi">QuotationAdminApi</h1>

## QuotationAdminApi_GetQuotations

<a id="opIdQuotationAdminApi_GetQuotations"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/quotation/list \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/quotation/list HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/quotation/list',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/quotation/list',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/quotation/list', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/quotation/list', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/quotation/list");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/quotation/list", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/quotation/list`

<h3 id="quotationadminapi_getquotations-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|keyword|query|string|false|none|
|branchId|query|integer(int32)|false|none|
|status|query|string|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="quotationadminapi_getquotations-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="quotationadminapi_getquotations-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## Lấy danh sách báo giá có phân trang.

<a id="opIdQuotationAdminApi_GetQuotationsPaged"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/quotation/paged \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/quotation/paged HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/quotation/paged',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/quotation/paged',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/quotation/paged', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/quotation/paged', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/quotation/paged");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/quotation/paged", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/quotation/paged`

<h3 id="lấy-danh-sách-báo-giá-có-phân-trang.-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|keyword|query|string|false|none|
|branchId|query|integer(int32)|false|none|
|status|query|string|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="lấy-danh-sách-báo-giá-có-phân-trang.-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="lấy-danh-sách-báo-giá-có-phân-trang.-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## QuotationAdminApi_GetQuotationDetail

<a id="opIdQuotationAdminApi_GetQuotationDetail"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/quotation/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/quotation/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/quotation/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/quotation/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/quotation/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/quotation/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/quotation/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/quotation/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/quotation/{id}`

<h3 id="quotationadminapi_getquotationdetail-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="quotationadminapi_getquotationdetail-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="quotationadminapi_getquotationdetail-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## QuotationAdminApi_UpdateQuotation

<a id="opIdQuotationAdminApi_UpdateQuotation"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/quotation/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/quotation/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "QuotationId": 0,
  "QuotationCode": "string",
  "BranchId": 0,
  "BranchName": "string",
  "CustomerId": 0,
  "CustomerName": "string",
  "CustomerPhone": "string",
  "CustomerAddress": "string",
  "UserId": 0,
  "CreatorName": "string",
  "IssueDate": "2019-08-24T14:15:22Z",
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "TotalAmount": 0.1,
  "DiscountAmount": 0.1,
  "ManualDiscount": 0.1,
  "DiscountReason": "string",
  "Status": "string",
  "Note": "string",
  "ConvertedOrderId": 0,
  "Details": [
    {
      "DetailId": 0,
      "ProductId": 0,
      "ProductName": "string",
      "ProductImage": "string",
      "Unit": "string",
      "Quantity": 0,
      "UnitPrice": 0.1,
      "Discount": 0.1,
      "TotalPrice": 0.1,
      "Note": "string"
    }
  ]
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/quotation/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/quotation/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/quotation/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/quotation/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/quotation/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/quotation/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/quotation/update`

> Body parameter

```json
{
  "QuotationId": 0,
  "QuotationCode": "string",
  "BranchId": 0,
  "BranchName": "string",
  "CustomerId": 0,
  "CustomerName": "string",
  "CustomerPhone": "string",
  "CustomerAddress": "string",
  "UserId": 0,
  "CreatorName": "string",
  "IssueDate": "2019-08-24T14:15:22Z",
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "TotalAmount": 0.1,
  "DiscountAmount": 0.1,
  "ManualDiscount": 0.1,
  "DiscountReason": "string",
  "Status": "string",
  "Note": "string",
  "ConvertedOrderId": 0,
  "Details": [
    {
      "DetailId": 0,
      "ProductId": 0,
      "ProductName": "string",
      "ProductImage": "string",
      "Unit": "string",
      "Quantity": 0,
      "UnitPrice": 0.1,
      "Discount": 0.1,
      "TotalPrice": 0.1,
      "Note": "string"
    }
  ]
}
```

```
QuotationId: 0
QuotationCode: string
BranchId: 0
BranchName: string
CustomerId: 0
CustomerName: string
CustomerPhone: string
CustomerAddress: string
UserId: 0
CreatorName: string
IssueDate: 2019-08-24T14:15:22Z
ExpiryDate: 2019-08-24T14:15:22Z
TotalAmount: 0.1
DiscountAmount: 0.1
ManualDiscount: 0.1
DiscountReason: string
Status: string
Note: string
ConvertedOrderId: 0
Details:
  - DetailId: 0
    ProductId: 0
    ProductName: string
    ProductImage: string
    Unit: string
    Quantity: 0
    UnitPrice: 0.1
    Discount: 0.1
    TotalPrice: 0.1
    Note: string

```

```yaml
QuotationId: 0
QuotationCode: string
BranchId: 0
BranchName: string
CustomerId: 0
CustomerName: string
CustomerPhone: string
CustomerAddress: string
UserId: 0
CreatorName: string
IssueDate: 2019-08-24T14:15:22Z
ExpiryDate: 2019-08-24T14:15:22Z
TotalAmount: 0.1
DiscountAmount: 0.1
ManualDiscount: 0.1
DiscountReason: string
Status: string
Note: string
ConvertedOrderId: 0
Details:
  - DetailId: 0
    ProductId: 0
    ProductName: string
    ProductImage: string
    Unit: string
    Quantity: 0
    UnitPrice: 0.1
    Discount: 0.1
    TotalPrice: 0.1
    Note: string

```

<h3 id="quotationadminapi_updatequotation-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|query|integer(int32)|false|none|
|body|body|[QuotationDTO](#schemaquotationdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="quotationadminapi_updatequotation-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="quotationadminapi_updatequotation-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## QuotationAdminApi_DeleteQuotation

<a id="opIdQuotationAdminApi_DeleteQuotation"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/quotation/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/quotation/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/quotation/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/quotation/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/quotation/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/quotation/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/quotation/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/quotation/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/quotation/delete/{id}`

<h3 id="quotationadminapi_deletequotation-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="quotationadminapi_deletequotation-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="quotationadminapi_deletequotation-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## QuotationAdminApi_SaveQuotation

<a id="opIdQuotationAdminApi_SaveQuotation"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/quotation/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/quotation/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "QuotationId": 0,
  "QuotationCode": "string",
  "BranchId": 0,
  "BranchName": "string",
  "CustomerId": 0,
  "CustomerName": "string",
  "CustomerPhone": "string",
  "CustomerAddress": "string",
  "UserId": 0,
  "CreatorName": "string",
  "IssueDate": "2019-08-24T14:15:22Z",
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "TotalAmount": 0.1,
  "DiscountAmount": 0.1,
  "ManualDiscount": 0.1,
  "DiscountReason": "string",
  "Status": "string",
  "Note": "string",
  "ConvertedOrderId": 0,
  "Details": [
    {
      "DetailId": 0,
      "ProductId": 0,
      "ProductName": "string",
      "ProductImage": "string",
      "Unit": "string",
      "Quantity": 0,
      "UnitPrice": 0.1,
      "Discount": 0.1,
      "TotalPrice": 0.1,
      "Note": "string"
    }
  ]
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/quotation/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/quotation/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/quotation/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/quotation/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/quotation/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/quotation/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/quotation/create`

> Body parameter

```json
{
  "QuotationId": 0,
  "QuotationCode": "string",
  "BranchId": 0,
  "BranchName": "string",
  "CustomerId": 0,
  "CustomerName": "string",
  "CustomerPhone": "string",
  "CustomerAddress": "string",
  "UserId": 0,
  "CreatorName": "string",
  "IssueDate": "2019-08-24T14:15:22Z",
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "TotalAmount": 0.1,
  "DiscountAmount": 0.1,
  "ManualDiscount": 0.1,
  "DiscountReason": "string",
  "Status": "string",
  "Note": "string",
  "ConvertedOrderId": 0,
  "Details": [
    {
      "DetailId": 0,
      "ProductId": 0,
      "ProductName": "string",
      "ProductImage": "string",
      "Unit": "string",
      "Quantity": 0,
      "UnitPrice": 0.1,
      "Discount": 0.1,
      "TotalPrice": 0.1,
      "Note": "string"
    }
  ]
}
```

```
QuotationId: 0
QuotationCode: string
BranchId: 0
BranchName: string
CustomerId: 0
CustomerName: string
CustomerPhone: string
CustomerAddress: string
UserId: 0
CreatorName: string
IssueDate: 2019-08-24T14:15:22Z
ExpiryDate: 2019-08-24T14:15:22Z
TotalAmount: 0.1
DiscountAmount: 0.1
ManualDiscount: 0.1
DiscountReason: string
Status: string
Note: string
ConvertedOrderId: 0
Details:
  - DetailId: 0
    ProductId: 0
    ProductName: string
    ProductImage: string
    Unit: string
    Quantity: 0
    UnitPrice: 0.1
    Discount: 0.1
    TotalPrice: 0.1
    Note: string

```

```yaml
QuotationId: 0
QuotationCode: string
BranchId: 0
BranchName: string
CustomerId: 0
CustomerName: string
CustomerPhone: string
CustomerAddress: string
UserId: 0
CreatorName: string
IssueDate: 2019-08-24T14:15:22Z
ExpiryDate: 2019-08-24T14:15:22Z
TotalAmount: 0.1
DiscountAmount: 0.1
ManualDiscount: 0.1
DiscountReason: string
Status: string
Note: string
ConvertedOrderId: 0
Details:
  - DetailId: 0
    ProductId: 0
    ProductName: string
    ProductImage: string
    Unit: string
    Quantity: 0
    UnitPrice: 0.1
    Discount: 0.1
    TotalPrice: 0.1
    Note: string

```

<h3 id="quotationadminapi_savequotation-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[QuotationDTO](#schemaquotationdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="quotationadminapi_savequotation-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="quotationadminapi_savequotation-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## QuotationAdminApi_ChangeStatus

<a id="opIdQuotationAdminApi_ChangeStatus"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/quotation/{id}/status \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/quotation/{id}/status HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "Status": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/quotation/{id}/status',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/quotation/{id}/status',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/quotation/{id}/status', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/quotation/{id}/status', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/quotation/{id}/status");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/quotation/{id}/status", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/quotation/{id}/status`

> Body parameter

```json
{
  "Status": "string"
}
```

```
Status: string

```

```yaml
Status: string

```

<h3 id="quotationadminapi_changestatus-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[ChangeStatusModel](#schemachangestatusmodel)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="quotationadminapi_changestatus-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="quotationadminapi_changestatus-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## QuotationAdminApi_ConvertToOrder

<a id="opIdQuotationAdminApi_ConvertToOrder"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/quotation/{id}/convert-to-order \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/quotation/{id}/convert-to-order HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/quotation/{id}/convert-to-order',
{
  method: 'POST',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/quotation/{id}/convert-to-order',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/quotation/{id}/convert-to-order', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/quotation/{id}/convert-to-order', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/quotation/{id}/convert-to-order");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/quotation/{id}/convert-to-order", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/quotation/{id}/convert-to-order`

<h3 id="quotationadminapi_converttoorder-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|userId|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="quotationadminapi_converttoorder-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="quotationadminapi_converttoorder-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-salesadminapi">SalesAdminApi</h1>

## SalesAdminApi_GetAllOrders

<a id="opIdSalesAdminApi_GetAllOrders"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/orders/all \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/orders/all HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/all',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/orders/all',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/orders/all', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/orders/all', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/all");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/orders/all", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/orders/all`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getallorders-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getallorders-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_GetOrders

<a id="opIdSalesAdminApi_GetOrders"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/orders \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/orders HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/orders',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/orders', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/orders', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/orders", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/orders`

<h3 id="salesadminapi_getorders-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|userId|query|integer(int32)|false|none|
|branchId|query|integer(int32)|false|none|
|status|query|string|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|
|keyword|query|string|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getorders-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getorders-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_GetOrderDetail

<a id="opIdSalesAdminApi_GetOrderDetail"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/orders/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/orders/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/orders/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/orders/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/orders/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/orders/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/orders/{id}`

<h3 id="salesadminapi_getorderdetail-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getorderdetail-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getorderdetail-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_GetFullOrdersByBranch

<a id="opIdSalesAdminApi_GetFullOrdersByBranch"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/orders/full-by-branch \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/orders/full-by-branch HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/full-by-branch',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/orders/full-by-branch',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/orders/full-by-branch', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/orders/full-by-branch', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/full-by-branch");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/orders/full-by-branch", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/orders/full-by-branch`

<h3 id="salesadminapi_getfullordersbybranch-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|query|integer(int32)|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getfullordersbybranch-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getfullordersbybranch-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_GetOrdersByBranch

<a id="opIdSalesAdminApi_GetOrdersByBranch"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/orders/by-branch \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/orders/by-branch HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/by-branch',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/orders/by-branch',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/orders/by-branch', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/orders/by-branch', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/by-branch");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/orders/by-branch", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/orders/by-branch`

<h3 id="salesadminapi_getordersbybranch-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|branchId|query|integer(int32)|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getordersbybranch-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getordersbybranch-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_GetOrderDetails

<a id="opIdSalesAdminApi_GetOrderDetails"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/orders/{orderId}/details \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/orders/{orderId}/details HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/{orderId}/details',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/orders/{orderId}/details',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/orders/{orderId}/details', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/orders/{orderId}/details', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/{orderId}/details");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/orders/{orderId}/details", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/orders/{orderId}/details`

<h3 id="salesadminapi_getorderdetails-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|orderId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getorderdetails-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getorderdetails-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_UpdateOrderStatus

<a id="opIdSalesAdminApi_UpdateOrderStatus"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/sales/orders/{orderId}/status \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/sales/orders/{orderId}/status HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "Status": "string",
  "ConnectionId": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/{orderId}/status',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/sales/orders/{orderId}/status',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/sales/orders/{orderId}/status', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/sales/orders/{orderId}/status', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/{orderId}/status");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/sales/orders/{orderId}/status", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/sales/orders/{orderId}/status`

> Body parameter

```json
{
  "Status": "string",
  "ConnectionId": "string"
}
```

```
Status: string
ConnectionId: string

```

```yaml
Status: string
ConnectionId: string

```

<h3 id="salesadminapi_updateorderstatus-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|orderId|path|integer(int32)|true|none|
|body|body|[UpdateStatusModel](#schemaupdatestatusmodel)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_updateorderstatus-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_updateorderstatus-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_CreateOrder

<a id="opIdSalesAdminApi_CreateOrder"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/sales/orders/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/sales/orders/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "OrderId": 0,
  "CustomerId": 0,
  "UserId": 0,
  "BranchId": 0,
  "OrderDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "TotalAmount": 0.1,
  "DeliveryAddressId": 0,
  "OrderDetails": [
    {
      "OrderDetailId": 0,
      "OrderId": 0,
      "ProductId": 0,
      "Quantity": 0,
      "UnitPrice": 0.1,
      "Discount": 0.1,
      "ProductName": "string",
      "CustomerId": 0,
      "QuantityReturned": 0
    }
  ],
  "PaymentMethod": "string",
  "PaymentStatus": "string",
  "CustomerName": "string",
  "UserName": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/sales/orders/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/sales/orders/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/sales/orders/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/sales/orders/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/sales/orders/create`

> Body parameter

```json
{
  "OrderId": 0,
  "CustomerId": 0,
  "UserId": 0,
  "BranchId": 0,
  "OrderDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "TotalAmount": 0.1,
  "DeliveryAddressId": 0,
  "OrderDetails": [
    {
      "OrderDetailId": 0,
      "OrderId": 0,
      "ProductId": 0,
      "Quantity": 0,
      "UnitPrice": 0.1,
      "Discount": 0.1,
      "ProductName": "string",
      "CustomerId": 0,
      "QuantityReturned": 0
    }
  ],
  "PaymentMethod": "string",
  "PaymentStatus": "string",
  "CustomerName": "string",
  "UserName": "string"
}
```

```
OrderId: 0
CustomerId: 0
UserId: 0
BranchId: 0
OrderDate: 2019-08-24T14:15:22Z
Status: string
TotalAmount: 0.1
DeliveryAddressId: 0
OrderDetails:
  - OrderDetailId: 0
    OrderId: 0
    ProductId: 0
    Quantity: 0
    UnitPrice: 0.1
    Discount: 0.1
    ProductName: string
    CustomerId: 0
    QuantityReturned: 0
PaymentMethod: string
PaymentStatus: string
CustomerName: string
UserName: string

```

```yaml
OrderId: 0
CustomerId: 0
UserId: 0
BranchId: 0
OrderDate: 2019-08-24T14:15:22Z
Status: string
TotalAmount: 0.1
DeliveryAddressId: 0
OrderDetails:
  - OrderDetailId: 0
    OrderId: 0
    ProductId: 0
    Quantity: 0
    UnitPrice: 0.1
    Discount: 0.1
    ProductName: string
    CustomerId: 0
    QuantityReturned: 0
PaymentMethod: string
PaymentStatus: string
CustomerName: string
UserName: string

```

<h3 id="salesadminapi_createorder-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[OrderDTO](#schemaorderdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_createorder-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_createorder-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_Checkout

<a id="opIdSalesAdminApi_Checkout"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/sales/orders/checkout \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/sales/orders/checkout HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "UserId": 0,
  "BranchId": 0,
  "CustomerId": 0,
  "PointsUsed": 0,
  "PointsEarned": 0,
  "TotalAmount": 0.1,
  "ManualDiscount": 0.1,
  "ManualDiscountReason": "string",
  "ParentOrderId": 0,
  "Status": "string",
  "PaymentMethod": "string",
  "ConnectionId": "string",
  "Items": [
    {
      "ProductId": 0,
      "Quantity": 0,
      "Price": 0.1,
      "Discount": 0.1
    }
  ]
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/checkout',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/sales/orders/checkout',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/sales/orders/checkout', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/sales/orders/checkout', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/checkout");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/sales/orders/checkout", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/sales/orders/checkout`

> Body parameter

```json
{
  "UserId": 0,
  "BranchId": 0,
  "CustomerId": 0,
  "PointsUsed": 0,
  "PointsEarned": 0,
  "TotalAmount": 0.1,
  "ManualDiscount": 0.1,
  "ManualDiscountReason": "string",
  "ParentOrderId": 0,
  "Status": "string",
  "PaymentMethod": "string",
  "ConnectionId": "string",
  "Items": [
    {
      "ProductId": 0,
      "Quantity": 0,
      "Price": 0.1,
      "Discount": 0.1
    }
  ]
}
```

```
UserId: 0
BranchId: 0
CustomerId: 0
PointsUsed: 0
PointsEarned: 0
TotalAmount: 0.1
ManualDiscount: 0.1
ManualDiscountReason: string
ParentOrderId: 0
Status: string
PaymentMethod: string
ConnectionId: string
Items:
  - ProductId: 0
    Quantity: 0
    Price: 0.1
    Discount: 0.1

```

```yaml
UserId: 0
BranchId: 0
CustomerId: 0
PointsUsed: 0
PointsEarned: 0
TotalAmount: 0.1
ManualDiscount: 0.1
ManualDiscountReason: string
ParentOrderId: 0
Status: string
PaymentMethod: string
ConnectionId: string
Items:
  - ProductId: 0
    Quantity: 0
    Price: 0.1
    Discount: 0.1

```

<h3 id="salesadminapi_checkout-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[CheckoutRequestDTO](#schemacheckoutrequestdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_checkout-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_checkout-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_CreateOrderDetail

<a id="opIdSalesAdminApi_CreateOrderDetail"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/sales/orders/details/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/sales/orders/details/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "OrderDetailId": 0,
  "OrderId": 0,
  "ProductId": 0,
  "Quantity": 0,
  "UnitPrice": 0.1,
  "Discount": 0.1,
  "ProductName": "string",
  "CustomerId": 0,
  "QuantityReturned": 0
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/details/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/sales/orders/details/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/sales/orders/details/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/sales/orders/details/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/details/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/sales/orders/details/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/sales/orders/details/create`

> Body parameter

```json
{
  "OrderDetailId": 0,
  "OrderId": 0,
  "ProductId": 0,
  "Quantity": 0,
  "UnitPrice": 0.1,
  "Discount": 0.1,
  "ProductName": "string",
  "CustomerId": 0,
  "QuantityReturned": 0
}
```

```
OrderDetailId: 0
OrderId: 0
ProductId: 0
Quantity: 0
UnitPrice: 0.1
Discount: 0.1
ProductName: string
CustomerId: 0
QuantityReturned: 0

```

```yaml
OrderDetailId: 0
OrderId: 0
ProductId: 0
Quantity: 0
UnitPrice: 0.1
Discount: 0.1
ProductName: string
CustomerId: 0
QuantityReturned: 0

```

<h3 id="salesadminapi_createorderdetail-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[OrderDetailDTO](#schemaorderdetaildto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_createorderdetail-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_createorderdetail-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_UpdateOrderDetail

<a id="opIdSalesAdminApi_UpdateOrderDetail"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/sales/orders/details/{id} \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/sales/orders/details/{id} HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "OrderDetailId": 0,
  "OrderId": 0,
  "ProductId": 0,
  "Quantity": 0,
  "UnitPrice": 0.1,
  "Discount": 0.1,
  "ProductName": "string",
  "CustomerId": 0,
  "QuantityReturned": 0
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/details/{id}',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/sales/orders/details/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/sales/orders/details/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/sales/orders/details/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/details/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/sales/orders/details/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/sales/orders/details/{id}`

> Body parameter

```json
{
  "OrderDetailId": 0,
  "OrderId": 0,
  "ProductId": 0,
  "Quantity": 0,
  "UnitPrice": 0.1,
  "Discount": 0.1,
  "ProductName": "string",
  "CustomerId": 0,
  "QuantityReturned": 0
}
```

```
OrderDetailId: 0
OrderId: 0
ProductId: 0
Quantity: 0
UnitPrice: 0.1
Discount: 0.1
ProductName: string
CustomerId: 0
QuantityReturned: 0

```

```yaml
OrderDetailId: 0
OrderId: 0
ProductId: 0
Quantity: 0
UnitPrice: 0.1
Discount: 0.1
ProductName: string
CustomerId: 0
QuantityReturned: 0

```

<h3 id="salesadminapi_updateorderdetail-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|
|body|body|[OrderDetailDTO](#schemaorderdetaildto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_updateorderdetail-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_updateorderdetail-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_DeleteOrderDetail

<a id="opIdSalesAdminApi_DeleteOrderDetail"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/sales/orders/details/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/sales/orders/details/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/details/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/sales/orders/details/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/sales/orders/details/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/sales/orders/details/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/details/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/sales/orders/details/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/sales/orders/details/delete/{id}`

<h3 id="salesadminapi_deleteorderdetail-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_deleteorderdetail-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_deleteorderdetail-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_DeleteOrder

<a id="opIdSalesAdminApi_DeleteOrder"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/sales/orders/{orderId} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/sales/orders/{orderId} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/{orderId}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/sales/orders/{orderId}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/sales/orders/{orderId}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/sales/orders/{orderId}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/{orderId}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/sales/orders/{orderId}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/sales/orders/{orderId}`

<h3 id="salesadminapi_deleteorder-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|orderId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_deleteorder-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_deleteorder-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_GetPaymentsByOrder

<a id="opIdSalesAdminApi_GetPaymentsByOrder"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/orders/{orderId}/payments \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/orders/{orderId}/payments HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/{orderId}/payments',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/orders/{orderId}/payments',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/orders/{orderId}/payments', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/orders/{orderId}/payments', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/{orderId}/payments");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/orders/{orderId}/payments", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/orders/{orderId}/payments`

<h3 id="salesadminapi_getpaymentsbyorder-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|orderId|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getpaymentsbyorder-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getpaymentsbyorder-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_CreatePayOSLink

<a id="opIdSalesAdminApi_CreatePayOSLink"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/sales/orders/create-payos-link \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/sales/orders/create-payos-link HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "Amount": 0,
  "Description": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/create-payos-link',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/sales/orders/create-payos-link',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/sales/orders/create-payos-link', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/sales/orders/create-payos-link', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/create-payos-link");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/sales/orders/create-payos-link", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/sales/orders/create-payos-link`

> Body parameter

```json
{
  "Amount": 0,
  "Description": "string"
}
```

```
Amount: 0
Description: string

```

```yaml
Amount: 0
Description: string

```

<h3 id="salesadminapi_createpayoslink-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[CreateQrRequestDTO](#schemacreateqrrequestdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_createpayoslink-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_createpayoslink-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_CheckPayOSStatus

<a id="opIdSalesAdminApi_CheckPayOSStatus"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/orders/check-payos-status/{orderCode} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/orders/check-payos-status/{orderCode} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/orders/check-payos-status/{orderCode}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/orders/check-payos-status/{orderCode}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/orders/check-payos-status/{orderCode}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/orders/check-payos-status/{orderCode}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/orders/check-payos-status/{orderCode}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/orders/check-payos-status/{orderCode}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/orders/check-payos-status/{orderCode}`

<h3 id="salesadminapi_checkpayosstatus-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|orderCode|path|integer(int64)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_checkpayosstatus-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_checkpayosstatus-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_GetAllPromotions

<a id="opIdSalesAdminApi_GetAllPromotions"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/promotions \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/promotions HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/promotions',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/promotions',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/promotions', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/promotions', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/promotions");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/promotions", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/promotions`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getallpromotions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getallpromotions-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_SearchPromotions

<a id="opIdSalesAdminApi_SearchPromotions"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/promotions/search \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/promotions/search HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/promotions/search',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/promotions/search',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/promotions/search', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/promotions/search', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/promotions/search");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/promotions/search", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/promotions/search`

<h3 id="salesadminapi_searchpromotions-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|keyword|query|string|false|none|
|categoryId|query|integer(int32)|false|none|
|discountType|query|string|false|none|
|cusType|query|string|false|none|
|fromDate|query|string(date-time)|false|none|
|toDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_searchpromotions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_searchpromotions-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_GetPromotionById

<a id="opIdSalesAdminApi_GetPromotionById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/promotions/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/promotions/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/promotions/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/promotions/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/promotions/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/promotions/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/promotions/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/promotions/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/promotions/{id}`

<h3 id="salesadminapi_getpromotionbyid-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getpromotionbyid-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getpromotionbyid-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_UpdatePromotion

<a id="opIdSalesAdminApi_UpdatePromotion"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/sales/promotions/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/sales/promotions/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "PromotionId": 0,
  "Name": "string",
  "DiscountType": "string",
  "DiscountValue": 0.1,
  "CategoryId": 0,
  "CustomerType": "string",
  "StartDate": "2019-08-24T14:15:22Z",
  "EndDate": "2019-08-24T14:15:22Z",
  "Priority": 0,
  "IsActive": true,
  "CategoryName": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/promotions/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/sales/promotions/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/sales/promotions/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/sales/promotions/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/promotions/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/sales/promotions/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/sales/promotions/update`

> Body parameter

```json
{
  "PromotionId": 0,
  "Name": "string",
  "DiscountType": "string",
  "DiscountValue": 0.1,
  "CategoryId": 0,
  "CustomerType": "string",
  "StartDate": "2019-08-24T14:15:22Z",
  "EndDate": "2019-08-24T14:15:22Z",
  "Priority": 0,
  "IsActive": true,
  "CategoryName": "string"
}
```

```
PromotionId: 0
Name: string
DiscountType: string
DiscountValue: 0.1
CategoryId: 0
CustomerType: string
StartDate: 2019-08-24T14:15:22Z
EndDate: 2019-08-24T14:15:22Z
Priority: 0
IsActive: true
CategoryName: string

```

```yaml
PromotionId: 0
Name: string
DiscountType: string
DiscountValue: 0.1
CategoryId: 0
CustomerType: string
StartDate: 2019-08-24T14:15:22Z
EndDate: 2019-08-24T14:15:22Z
Priority: 0
IsActive: true
CategoryName: string

```

<h3 id="salesadminapi_updatepromotion-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|query|integer(int32)|false|none|
|body|body|[PromotionDTO](#schemapromotiondto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_updatepromotion-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_updatepromotion-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_DeletePromotion

<a id="opIdSalesAdminApi_DeletePromotion"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/sales/promotions/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/sales/promotions/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/promotions/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/sales/promotions/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/sales/promotions/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/sales/promotions/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/promotions/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/sales/promotions/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/sales/promotions/delete/{id}`

<h3 id="salesadminapi_deletepromotion-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_deletepromotion-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_deletepromotion-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_GetActivePromotions

<a id="opIdSalesAdminApi_GetActivePromotions"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/sales/promotions/active \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/sales/promotions/active HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/promotions/active',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/sales/promotions/active',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/sales/promotions/active', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/sales/promotions/active', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/promotions/active");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/sales/promotions/active", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/sales/promotions/active`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_getactivepromotions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_getactivepromotions-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## SalesAdminApi_CreatePromotion

<a id="opIdSalesAdminApi_CreatePromotion"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/sales/promotions/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/sales/promotions/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "PromotionId": 0,
  "Name": "string",
  "DiscountType": "string",
  "DiscountValue": 0.1,
  "CategoryId": 0,
  "CustomerType": "string",
  "StartDate": "2019-08-24T14:15:22Z",
  "EndDate": "2019-08-24T14:15:22Z",
  "Priority": 0,
  "IsActive": true,
  "CategoryName": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/sales/promotions/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/sales/promotions/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/sales/promotions/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/sales/promotions/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/sales/promotions/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/sales/promotions/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/sales/promotions/create`

> Body parameter

```json
{
  "PromotionId": 0,
  "Name": "string",
  "DiscountType": "string",
  "DiscountValue": 0.1,
  "CategoryId": 0,
  "CustomerType": "string",
  "StartDate": "2019-08-24T14:15:22Z",
  "EndDate": "2019-08-24T14:15:22Z",
  "Priority": 0,
  "IsActive": true,
  "CategoryName": "string"
}
```

```
PromotionId: 0
Name: string
DiscountType: string
DiscountValue: 0.1
CategoryId: 0
CustomerType: string
StartDate: 2019-08-24T14:15:22Z
EndDate: 2019-08-24T14:15:22Z
Priority: 0
IsActive: true
CategoryName: string

```

```yaml
PromotionId: 0
Name: string
DiscountType: string
DiscountValue: 0.1
CategoryId: 0
CustomerType: string
StartDate: 2019-08-24T14:15:22Z
EndDate: 2019-08-24T14:15:22Z
Priority: 0
IsActive: true
CategoryName: string

```

<h3 id="salesadminapi_createpromotion-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[PromotionDTO](#schemapromotiondto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="salesadminapi_createpromotion-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="salesadminapi_createpromotion-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-test">Test</h1>

## Test_Hello

<a id="opIdTest_Hello"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/test/hello \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/test/hello HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/test/hello',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/test/hello',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/test/hello', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/test/hello', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/test/hello");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/test/hello", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/test/hello`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="test_hello-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="test_hello-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

<h1 id="emarket-api-useradminapi">UserAdminApi</h1>

## UserAdminApi_Login

<a id="opIdUserAdminApi_Login"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/auth/login \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/auth/login HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "EmailOrUsername": "string",
  "Password": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/auth/login',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/auth/login',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/auth/login', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/auth/login', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/auth/login");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/auth/login", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/auth/login`

> Body parameter

```json
{
  "EmailOrUsername": "string",
  "Password": "string"
}
```

```
EmailOrUsername: string
Password: string

```

```yaml
EmailOrUsername: string
Password: string

```

<h3 id="useradminapi_login-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[LoginRequestDTO](#schemaloginrequestdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_login-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_login-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_ExternalLogin

<a id="opIdUserAdminApi_ExternalLogin"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/auth/google-login \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/auth/google-login HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "Provider": "string",
  "Email": "string",
  "IdToken": "string",
  "FullName": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/auth/google-login',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/auth/google-login',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/auth/google-login', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/auth/google-login', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/auth/google-login");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/auth/google-login", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/auth/google-login`

> Body parameter

```json
{
  "Provider": "string",
  "Email": "string",
  "IdToken": "string",
  "FullName": "string"
}
```

```
Provider: string
Email: string
IdToken: string
FullName: string

```

```yaml
Provider: string
Email: string
IdToken: string
FullName: string

```

<h3 id="useradminapi_externallogin-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ExternalLoginRequestDTO](#schemaexternalloginrequestdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_externallogin-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_externallogin-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_RequestOtp

<a id="opIdUserAdminApi_RequestOtp"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/auth/request-otp \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/auth/request-otp HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "Email": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/auth/request-otp',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/auth/request-otp',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/auth/request-otp', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/auth/request-otp', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/auth/request-otp");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/auth/request-otp", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/auth/request-otp`

> Body parameter

```json
{
  "Email": "string"
}
```

```
Email: string

```

```yaml
Email: string

```

<h3 id="useradminapi_requestotp-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[RequestOtpModel](#schemarequestotpmodel)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_requestotp-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_requestotp-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_ResetPassword

<a id="opIdUserAdminApi_ResetPassword"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/auth/reset-password \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/auth/reset-password HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "Email": "string",
  "Otp": "string",
  "NewPassword": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/auth/reset-password',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/auth/reset-password',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/auth/reset-password', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/auth/reset-password', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/auth/reset-password");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/auth/reset-password", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/auth/reset-password`

> Body parameter

```json
{
  "Email": "string",
  "Otp": "string",
  "NewPassword": "string"
}
```

```
Email: string
Otp: string
NewPassword: string

```

```yaml
Email: string
Otp: string
NewPassword: string

```

<h3 id="useradminapi_resetpassword-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[ResetPasswordRequestDTO](#schemaresetpasswordrequestdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_resetpassword-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_resetpassword-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetAllUsers

<a id="opIdUserAdminApi_GetAllUsers"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/users \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/users HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/users',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/users', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/users', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/users", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/users`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getallusers-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getallusers-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_SearchUsers

<a id="opIdUserAdminApi_SearchUsers"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/users/paged \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/users/paged HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/paged',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/users/paged',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/users/paged', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/users/paged', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/paged");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/users/paged", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/users/paged`

<h3 id="useradminapi_searchusers-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|start|query|integer(int32)|false|none|
|length|query|integer(int32)|false|none|
|keyword|query|string|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_searchusers-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_searchusers-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetUserById

<a id="opIdUserAdminApi_GetUserById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/users/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/users/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/users/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/users/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/users/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/users/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/users/{id}`

<h3 id="useradminapi_getuserbyid-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getuserbyid-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getuserbyid-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_UpdateUser

<a id="opIdUserAdminApi_UpdateUser"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/users/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/users/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "UserId": 0,
  "Username": "string",
  "FullName": "string",
  "Email": "string",
  "Image": "string",
  "Phone": "string",
  "Status": "string",
  "SupplierId": 0,
  "BranchId": 0,
  "RoleId": 0,
  "Roles": [
    {
      "RoleId": 0,
      "Name": "string",
      "Permissions": [
        {
          "PermissionId": 0,
          "Name": "string",
          "Module": "string"
        }
      ]
    }
  ],
  "Permissions": [
    {
      "PermissionId": 0,
      "Name": "string",
      "Module": "string"
    }
  ],
  "IsAdmin": true,
  "IsSupplier": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/users/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/users/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/users/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/users/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/users/update`

> Body parameter

```json
{
  "UserId": 0,
  "Username": "string",
  "FullName": "string",
  "Email": "string",
  "Image": "string",
  "Phone": "string",
  "Status": "string",
  "SupplierId": 0,
  "BranchId": 0,
  "RoleId": 0,
  "Roles": [
    {
      "RoleId": 0,
      "Name": "string",
      "Permissions": [
        {
          "PermissionId": 0,
          "Name": "string",
          "Module": "string"
        }
      ]
    }
  ],
  "Permissions": [
    {
      "PermissionId": 0,
      "Name": "string",
      "Module": "string"
    }
  ],
  "IsAdmin": true,
  "IsSupplier": true
}
```

```
UserId: 0
Username: string
FullName: string
Email: string
Image: string
Phone: string
Status: string
SupplierId: 0
BranchId: 0
RoleId: 0
Roles:
  - RoleId: 0
    Name: string
    Permissions:
      - PermissionId: 0
        Name: string
        Module: string
Permissions:
  - PermissionId: 0
    Name: string
    Module: string
IsAdmin: true
IsSupplier: true

```

```yaml
UserId: 0
Username: string
FullName: string
Email: string
Image: string
Phone: string
Status: string
SupplierId: 0
BranchId: 0
RoleId: 0
Roles:
  - RoleId: 0
    Name: string
    Permissions:
      - PermissionId: 0
        Name: string
        Module: string
Permissions:
  - PermissionId: 0
    Name: string
    Module: string
IsAdmin: true
IsSupplier: true

```

<h3 id="useradminapi_updateuser-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|query|integer(int32)|false|none|
|body|body|[CurrentUserDTO](#schemacurrentuserdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_updateuser-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_updateuser-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_DeleteUser

<a id="opIdUserAdminApi_DeleteUser"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/user-management/users/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/user-management/users/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/user-management/users/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/user-management/users/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/user-management/users/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/user-management/users/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/user-management/users/delete/{id}`

<h3 id="useradminapi_deleteuser-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_deleteuser-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_deleteuser-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetUsersByIds

<a id="opIdUserAdminApi_GetUsersByIds"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/users/by-ids \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/users/by-ids HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '[
  0
]';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/by-ids',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/users/by-ids',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/users/by-ids', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/users/by-ids', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/by-ids");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/users/by-ids", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/users/by-ids`

> Body parameter

```json
[
  0
]
```

```
- 0

```

```yaml
- 0

```

<h3 id="useradminapi_getusersbyids-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|array[integer]|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getusersbyids-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getusersbyids-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetUserDict

<a id="opIdUserAdminApi_GetUserDict"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/users/dict \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/users/dict HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/dict',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/users/dict',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/users/dict', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/users/dict', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/dict");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/users/dict", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/users/dict`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getuserdict-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getuserdict-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetUserStats

<a id="opIdUserAdminApi_GetUserStats"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/users/stats \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/users/stats HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/stats',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/users/stats',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/users/stats', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/users/stats', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/stats");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/users/stats", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/users/stats`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getuserstats-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getuserstats-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_CountNewUsers

<a id="opIdUserAdminApi_CountNewUsers"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/users/count-new \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/users/count-new HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/count-new',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/users/count-new',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/users/count-new', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/users/count-new', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/count-new");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/users/count-new", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/users/count-new`

<h3 id="useradminapi_countnewusers-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|fromDate|query|string(date-time)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_countnewusers-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_countnewusers-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetRecentAvatars

<a id="opIdUserAdminApi_GetRecentAvatars"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/users/recent-avatars \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/users/recent-avatars HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/recent-avatars',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/users/recent-avatars',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/users/recent-avatars', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/users/recent-avatars', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/recent-avatars");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/users/recent-avatars", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/users/recent-avatars`

<h3 id="useradminapi_getrecentavatars-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|top|query|integer(int32)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getrecentavatars-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getrecentavatars-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetWarehouseManagerEmails

<a id="opIdUserAdminApi_GetWarehouseManagerEmails"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/users/warehouse-managers-emails \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/users/warehouse-managers-emails HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/warehouse-managers-emails',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/users/warehouse-managers-emails',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/users/warehouse-managers-emails', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/users/warehouse-managers-emails', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/warehouse-managers-emails");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/users/warehouse-managers-emails", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/users/warehouse-managers-emails`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getwarehousemanageremails-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getwarehousemanageremails-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_CreateUser

<a id="opIdUserAdminApi_CreateUser"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/users/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/users/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "UserId": 0,
  "Username": "string",
  "FullName": "string",
  "Email": "string",
  "Image": "string",
  "Phone": "string",
  "Status": "string",
  "SupplierId": 0,
  "BranchId": 0,
  "RoleId": 0,
  "Roles": [
    {
      "RoleId": 0,
      "Name": "string",
      "Permissions": [
        {
          "PermissionId": 0,
          "Name": "string",
          "Module": "string"
        }
      ]
    }
  ],
  "Permissions": [
    {
      "PermissionId": 0,
      "Name": "string",
      "Module": "string"
    }
  ],
  "IsAdmin": true,
  "IsSupplier": true
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/users/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/users/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/users/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/users/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/users/create`

> Body parameter

```json
{
  "UserId": 0,
  "Username": "string",
  "FullName": "string",
  "Email": "string",
  "Image": "string",
  "Phone": "string",
  "Status": "string",
  "SupplierId": 0,
  "BranchId": 0,
  "RoleId": 0,
  "Roles": [
    {
      "RoleId": 0,
      "Name": "string",
      "Permissions": [
        {
          "PermissionId": 0,
          "Name": "string",
          "Module": "string"
        }
      ]
    }
  ],
  "Permissions": [
    {
      "PermissionId": 0,
      "Name": "string",
      "Module": "string"
    }
  ],
  "IsAdmin": true,
  "IsSupplier": true
}
```

```
UserId: 0
Username: string
FullName: string
Email: string
Image: string
Phone: string
Status: string
SupplierId: 0
BranchId: 0
RoleId: 0
Roles:
  - RoleId: 0
    Name: string
    Permissions:
      - PermissionId: 0
        Name: string
        Module: string
Permissions:
  - PermissionId: 0
    Name: string
    Module: string
IsAdmin: true
IsSupplier: true

```

```yaml
UserId: 0
Username: string
FullName: string
Email: string
Image: string
Phone: string
Status: string
SupplierId: 0
BranchId: 0
RoleId: 0
Roles:
  - RoleId: 0
    Name: string
    Permissions:
      - PermissionId: 0
        Name: string
        Module: string
Permissions:
  - PermissionId: 0
    Name: string
    Module: string
IsAdmin: true
IsSupplier: true

```

<h3 id="useradminapi_createuser-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[CurrentUserDTO](#schemacurrentuserdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_createuser-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_createuser-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_UpdateUserEmail

<a id="opIdUserAdminApi_UpdateUserEmail"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/users/update-email \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/users/update-email HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "UserId": 0,
  "NewEmail": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/update-email',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/users/update-email',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/users/update-email', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/users/update-email', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/update-email");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/users/update-email", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/users/update-email`

> Body parameter

```json
{
  "UserId": 0,
  "NewEmail": "string"
}
```

```
UserId: 0
NewEmail: string

```

```yaml
UserId: 0
NewEmail: string

```

<h3 id="useradminapi_updateuseremail-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[UpdateEmailModel](#schemaupdateemailmodel)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_updateuseremail-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_updateuseremail-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_VerifyAndChangeEmail

<a id="opIdUserAdminApi_VerifyAndChangeEmail"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/users/verify-change-email \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/users/verify-change-email HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "UserId": 0,
  "CurrentEmail": "string",
  "Otp": "string",
  "NewEmail": "string"
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/users/verify-change-email',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/users/verify-change-email',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/users/verify-change-email', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/users/verify-change-email', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/users/verify-change-email");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/users/verify-change-email", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/users/verify-change-email`

> Body parameter

```json
{
  "UserId": 0,
  "CurrentEmail": "string",
  "Otp": "string",
  "NewEmail": "string"
}
```

```
UserId: 0
CurrentEmail: string
Otp: string
NewEmail: string

```

```yaml
UserId: 0
CurrentEmail: string
Otp: string
NewEmail: string

```

<h3 id="useradminapi_verifyandchangeemail-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[VerifyChangeEmailModel](#schemaverifychangeemailmodel)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_verifyandchangeemail-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_verifyandchangeemail-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetAllBranches

<a id="opIdUserAdminApi_GetAllBranches"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/branches \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/branches HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/branches',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/branches',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/branches', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/branches', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/branches");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/branches", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/branches`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getallbranches-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getallbranches-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_SearchBranches

<a id="opIdUserAdminApi_SearchBranches"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/branches/search \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/branches/search HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/branches/search',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/branches/search',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/branches/search', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/branches/search', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/branches/search");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/branches/search", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/branches/search`

<h3 id="useradminapi_searchbranches-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|name|query|string|false|none|
|lat|query|number(double)|false|none|
|lng|query|number(double)|false|none|
|maxDist|query|number(double)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_searchbranches-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_searchbranches-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetNearestBranch

<a id="opIdUserAdminApi_GetNearestBranch"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/branches/nearest?lat=0.1&lng=0.1 \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/branches/nearest?lat=0.1&lng=0.1 HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/branches/nearest?lat=0.1&lng=0.1',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/branches/nearest',
  params: {
  'lat' => 'number(double)',
'lng' => 'number(double)'
}, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/branches/nearest', params={
  'lat': '0.1',  'lng': '0.1'
}, headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/branches/nearest', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/branches/nearest?lat=0.1&lng=0.1");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/branches/nearest", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/branches/nearest`

<h3 id="useradminapi_getnearestbranch-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|lat|query|number(double)|true|none|
|lng|query|number(double)|true|none|
|maxDist|query|number(double)|false|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getnearestbranch-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getnearestbranch-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetBranchById

<a id="opIdUserAdminApi_GetBranchById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/branches/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/branches/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/branches/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/branches/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/branches/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/branches/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/branches/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/branches/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/branches/{id}`

<h3 id="useradminapi_getbranchbyid-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getbranchbyid-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getbranchbyid-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_UpdateBranch

<a id="opIdUserAdminApi_UpdateBranch"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/branches/update \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/branches/update HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true,
  "Distance": 0.1
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/branches/update',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/branches/update',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/branches/update', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/branches/update', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/branches/update");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/branches/update", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/branches/update`

> Body parameter

```json
{
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true,
  "Distance": 0.1
}
```

```
BranchId: 0
Name: string
Address: string
AddressUrl: string
Latitude: 0.1
Longitude: 0.1
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
CanBeDeleted: true
Distance: 0.1

```

```yaml
BranchId: 0
Name: string
Address: string
AddressUrl: string
Latitude: 0.1
Longitude: 0.1
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
CanBeDeleted: true
Distance: 0.1

```

<h3 id="useradminapi_updatebranch-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|query|integer(int32)|false|none|
|body|body|[BranchDTO](#schemabranchdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_updatebranch-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_updatebranch-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_DeleteBranch

<a id="opIdUserAdminApi_DeleteBranch"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/user-management/branches/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/user-management/branches/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/branches/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/user-management/branches/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/user-management/branches/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/user-management/branches/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/branches/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/user-management/branches/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/user-management/branches/delete/{id}`

<h3 id="useradminapi_deletebranch-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_deletebranch-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_deletebranch-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetBranchesByIds

<a id="opIdUserAdminApi_GetBranchesByIds"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/branches/by-ids \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/branches/by-ids HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '[
  0
]';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/branches/by-ids',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/branches/by-ids',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/branches/by-ids', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/branches/by-ids', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/branches/by-ids");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/branches/by-ids", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/branches/by-ids`

> Body parameter

```json
[
  0
]
```

```
- 0

```

```yaml
- 0

```

<h3 id="useradminapi_getbranchesbyids-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|array[integer]|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getbranchesbyids-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getbranchesbyids-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetBranchDict

<a id="opIdUserAdminApi_GetBranchDict"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/branches/dict \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/branches/dict HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/branches/dict',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/branches/dict',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/branches/dict', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/branches/dict', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/branches/dict");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/branches/dict", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/branches/dict`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getbranchdict-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getbranchdict-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_CreateBranch

<a id="opIdUserAdminApi_CreateBranch"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/branches/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/branches/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true,
  "Distance": 0.1
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/branches/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/branches/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/branches/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/branches/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/branches/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/branches/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/branches/create`

> Body parameter

```json
{
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true,
  "Distance": 0.1
}
```

```
BranchId: 0
Name: string
Address: string
AddressUrl: string
Latitude: 0.1
Longitude: 0.1
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
CanBeDeleted: true
Distance: 0.1

```

```yaml
BranchId: 0
Name: string
Address: string
AddressUrl: string
Latitude: 0.1
Longitude: 0.1
CreatedAt: 2019-08-24T14:15:22Z
UpdatedAt: 2019-08-24T14:15:22Z
CanBeDeleted: true
Distance: 0.1

```

<h3 id="useradminapi_createbranch-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[BranchDTO](#schemabranchdto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_createbranch-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_createbranch-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetAllRoles

<a id="opIdUserAdminApi_GetAllRoles"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/roles \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/roles HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/roles',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/roles',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/roles', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/roles', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/roles");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/roles", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/roles`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getallroles-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getallroles-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetRoleById

<a id="opIdUserAdminApi_GetRoleById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/roles/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/roles/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/roles/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/roles/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/roles/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/roles/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/roles/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/roles/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/roles/{id}`

<h3 id="useradminapi_getrolebyid-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getrolebyid-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getrolebyid-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_DeleteRole

<a id="opIdUserAdminApi_DeleteRole"></a>

> Code samples

```shell
# You can also use wget
curl -X DELETE https://localhost:44339/api/admin/user-management/roles/delete/{id} \
  -H 'Accept: application/json'

```

```http
DELETE https://localhost:44339/api/admin/user-management/roles/delete/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/roles/delete/{id}',
{
  method: 'DELETE',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.delete 'https://localhost:44339/api/admin/user-management/roles/delete/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.delete('https://localhost:44339/api/admin/user-management/roles/delete/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('DELETE','https://localhost:44339/api/admin/user-management/roles/delete/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/roles/delete/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("DELETE");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("DELETE", "https://localhost:44339/api/admin/user-management/roles/delete/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`DELETE /api/admin/user-management/roles/delete/{id}`

<h3 id="useradminapi_deleterole-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_deleterole-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_deleterole-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetRolePermissions

<a id="opIdUserAdminApi_GetRolePermissions"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/roles/{id}/permissions \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/roles/{id}/permissions HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/roles/{id}/permissions',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/roles/{id}/permissions',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/roles/{id}/permissions', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/roles/{id}/permissions', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/roles/{id}/permissions");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/roles/{id}/permissions", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/roles/{id}/permissions`

<h3 id="useradminapi_getrolepermissions-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getrolepermissions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getrolepermissions-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_CreateRole

<a id="opIdUserAdminApi_CreateRole"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/roles/create \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/roles/create HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "RoleId": 0,
  "Name": "string",
  "Permissions": [
    {
      "PermissionId": 0,
      "Name": "string",
      "Module": "string"
    }
  ]
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/roles/create',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/roles/create',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/roles/create', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/roles/create', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/roles/create");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/roles/create", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/roles/create`

> Body parameter

```json
{
  "RoleId": 0,
  "Name": "string",
  "Permissions": [
    {
      "PermissionId": 0,
      "Name": "string",
      "Module": "string"
    }
  ]
}
```

```
RoleId: 0
Name: string
Permissions:
  - PermissionId: 0
    Name: string
    Module: string

```

```yaml
RoleId: 0
Name: string
Permissions:
  - PermissionId: 0
    Name: string
    Module: string

```

<h3 id="useradminapi_createrole-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|body|body|[RoleDTO](#schemaroledto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_createrole-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_createrole-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_UpdateRolePermissions

<a id="opIdUserAdminApi_UpdateRolePermissions"></a>

> Code samples

```shell
# You can also use wget
curl -X POST https://localhost:44339/api/admin/user-management/roles/{roleId}/permissions \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json'

```

```http
POST https://localhost:44339/api/admin/user-management/roles/{roleId}/permissions HTTP/1.1
Host: localhost:44339
Content-Type: application/json
Accept: application/json

```

```javascript
const inputBody = '{
  "RoleId": 0,
  "PermissionIds": [
    0
  ]
}';
const headers = {
  'Content-Type':'application/json',
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/roles/{roleId}/permissions',
{
  method: 'POST',
  body: inputBody,
  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Content-Type' => 'application/json',
  'Accept' => 'application/json'
}

result = RestClient.post 'https://localhost:44339/api/admin/user-management/roles/{roleId}/permissions',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

r = requests.post('https://localhost:44339/api/admin/user-management/roles/{roleId}/permissions', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Content-Type' => 'application/json',
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('POST','https://localhost:44339/api/admin/user-management/roles/{roleId}/permissions', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/roles/{roleId}/permissions");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("POST");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Content-Type": []string{"application/json"},
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("POST", "https://localhost:44339/api/admin/user-management/roles/{roleId}/permissions", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`POST /api/admin/user-management/roles/{roleId}/permissions`

> Body parameter

```json
{
  "RoleId": 0,
  "PermissionIds": [
    0
  ]
}
```

```
RoleId: 0
PermissionIds:
  - 0

```

```yaml
RoleId: 0
PermissionIds:
  - 0

```

<h3 id="useradminapi_updaterolepermissions-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|roleId|path|integer(int32)|true|none|
|body|body|[RolePermissionUpdateDTO](#schemarolepermissionupdatedto)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_updaterolepermissions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_updaterolepermissions-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetAllPermissions

<a id="opIdUserAdminApi_GetAllPermissions"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/permissions \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/permissions HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/permissions',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/permissions',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/permissions', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/permissions', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/permissions");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/permissions", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/permissions`

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getallpermissions-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getallpermissions-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

## UserAdminApi_GetPermissionById

<a id="opIdUserAdminApi_GetPermissionById"></a>

> Code samples

```shell
# You can also use wget
curl -X GET https://localhost:44339/api/admin/user-management/permissions/{id} \
  -H 'Accept: application/json'

```

```http
GET https://localhost:44339/api/admin/user-management/permissions/{id} HTTP/1.1
Host: localhost:44339
Accept: application/json

```

```javascript

const headers = {
  'Accept':'application/json'
};

fetch('https://localhost:44339/api/admin/user-management/permissions/{id}',
{
  method: 'GET',

  headers: headers
})
.then(function(res) {
    return res.json();
}).then(function(body) {
    console.log(body);
});

```

```ruby
require 'rest-client'
require 'json'

headers = {
  'Accept' => 'application/json'
}

result = RestClient.get 'https://localhost:44339/api/admin/user-management/permissions/{id}',
  params: {
  }, headers: headers

p JSON.parse(result)

```

```python
import requests
headers = {
  'Accept': 'application/json'
}

r = requests.get('https://localhost:44339/api/admin/user-management/permissions/{id}', headers = headers)

print(r.json())

```

```php
<?php

require 'vendor/autoload.php';

$headers = array(
    'Accept' => 'application/json',
);

$client = new \GuzzleHttp\Client();

// Define array of request body.
$request_body = array();

try {
    $response = $client->request('GET','https://localhost:44339/api/admin/user-management/permissions/{id}', array(
        'headers' => $headers,
        'json' => $request_body,
       )
    );
    print_r($response->getBody()->getContents());
 }
 catch (\GuzzleHttp\Exception\BadResponseException $e) {
    // handle exception or api errors.
    print_r($e->getMessage());
 }

 // ...

```

```java
URL obj = new URL("https://localhost:44339/api/admin/user-management/permissions/{id}");
HttpURLConnection con = (HttpURLConnection) obj.openConnection();
con.setRequestMethod("GET");
int responseCode = con.getResponseCode();
BufferedReader in = new BufferedReader(
    new InputStreamReader(con.getInputStream()));
String inputLine;
StringBuffer response = new StringBuffer();
while ((inputLine = in.readLine()) != null) {
    response.append(inputLine);
}
in.close();
System.out.println(response.toString());

```

```go
package main

import (
       "bytes"
       "net/http"
)

func main() {

    headers := map[string][]string{
        "Accept": []string{"application/json"},
    }

    data := bytes.NewBuffer([]byte{jsonReq})
    req, err := http.NewRequest("GET", "https://localhost:44339/api/admin/user-management/permissions/{id}", data)
    req.Header = headers

    client := &http.Client{}
    resp, err := client.Do(req)
    // ...
}

```

`GET /api/admin/user-management/permissions/{id}`

<h3 id="useradminapi_getpermissionbyid-parameters">Parameters</h3>

|Name|In|Type|Required|Description|
|---|---|---|---|---|
|id|path|integer(int32)|true|none|

> Example responses

> 200 Response

```json
{}
```

```
{}
```

<h3 id="useradminapi_getpermissionbyid-responses">Responses</h3>

|Status|Meaning|Description|Schema|
|---|---|---|---|
|200|[OK](https://tools.ietf.org/html/rfc7231#section-6.3.1)|OK|Inline|

<h3 id="useradminapi_getpermissionbyid-responseschema">Response Schema</h3>

<aside class="success">
This operation does not require authentication
</aside>

# Schemas

<h2 id="tocS_RunAnalysisRequest">RunAnalysisRequest</h2>
<!-- backwards compatibility -->
<a id="schemarunanalysisrequest"></a>
<a id="schema_RunAnalysisRequest"></a>
<a id="tocSrunanalysisrequest"></a>
<a id="tocsrunanalysisrequest"></a>

```json
{
  "Mode": "string",
  "BranchId": 0
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|Mode|string|false|none|none|
|BranchId|integer(int32)|false|none|none|

<h2 id="tocS_CustomerUpdateDTO">CustomerUpdateDTO</h2>
<!-- backwards compatibility -->
<a id="schemacustomerupdatedto"></a>
<a id="schema_CustomerUpdateDTO"></a>
<a id="tocScustomerupdatedto"></a>
<a id="tocscustomerupdatedto"></a>

```json
{
  "CustomerId": 0,
  "FullName": "string",
  "Email": "string",
  "Phone": "string",
  "CustomerType": "string",
  "UserImg": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|CustomerId|integer(int32)|false|none|none|
|FullName|string|false|none|none|
|Email|string|false|none|none|
|Phone|string|false|none|none|
|CustomerType|string|false|none|none|
|UserImg|string|false|none|none|

<h2 id="tocS_CustomerCreateDTO">CustomerCreateDTO</h2>
<!-- backwards compatibility -->
<a id="schemacustomercreatedto"></a>
<a id="schema_CustomerCreateDTO"></a>
<a id="tocScustomercreatedto"></a>
<a id="tocscustomercreatedto"></a>

```json
{
  "FullName": "string",
  "Email": "string",
  "Phone": "string",
  "CustomerType": "string",
  "UserImg": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|FullName|string|false|none|none|
|Email|string|false|none|none|
|Phone|string|false|none|none|
|CustomerType|string|false|none|none|
|UserImg|string|false|none|none|

<h2 id="tocS_CustomerAddressUpdateDTO">CustomerAddressUpdateDTO</h2>
<!-- backwards compatibility -->
<a id="schemacustomeraddressupdatedto"></a>
<a id="schema_CustomerAddressUpdateDTO"></a>
<a id="tocScustomeraddressupdatedto"></a>
<a id="tocscustomeraddressupdatedto"></a>

```json
{
  "AddressId": 0,
  "CustomerId": 0,
  "FullAddress": "string",
  "AddressUrl": "string",
  "IsDefault": true
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|AddressId|integer(int32)|false|none|none|
|CustomerId|integer(int32)|false|none|none|
|FullAddress|string|false|none|none|
|AddressUrl|string|false|none|none|
|IsDefault|boolean|false|none|none|

<h2 id="tocS_CustomerAddressCreateDTO">CustomerAddressCreateDTO</h2>
<!-- backwards compatibility -->
<a id="schemacustomeraddresscreatedto"></a>
<a id="schema_CustomerAddressCreateDTO"></a>
<a id="tocScustomeraddresscreatedto"></a>
<a id="tocscustomeraddresscreatedto"></a>

```json
{
  "CustomerId": 0,
  "FullAddress": "string",
  "AddressUrl": "string",
  "IsDefault": true
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|CustomerId|integer(int32)|false|none|none|
|FullAddress|string|false|none|none|
|AddressUrl|string|false|none|none|
|IsDefault|boolean|false|none|none|

<h2 id="tocS_LoyaltyProgramDTO">LoyaltyProgramDTO</h2>
<!-- backwards compatibility -->
<a id="schemaloyaltyprogramdto"></a>
<a id="schema_LoyaltyProgramDTO"></a>
<a id="tocSloyaltyprogramdto"></a>
<a id="tocsloyaltyprogramdto"></a>

```json
{
  "LoyaltyId": 0,
  "CustomerId": 0,
  "OrderId": 0,
  "PointsEarned": 0,
  "PointsRedeemed": 0,
  "CreatedAt": "2019-08-24T14:15:22Z"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|LoyaltyId|integer(int32)|false|none|none|
|CustomerId|integer(int32)|false|none|none|
|OrderId|integer(int32)|false|none|none|
|PointsEarned|integer(int32)|false|none|none|
|PointsRedeemed|integer(int32)|false|none|none|
|CreatedAt|string(date-time)|false|none|none|

<h2 id="tocS_ExpenseDTO">ExpenseDTO</h2>
<!-- backwards compatibility -->
<a id="schemaexpensedto"></a>
<a id="schema_ExpenseDTO"></a>
<a id="tocSexpensedto"></a>
<a id="tocsexpensedto"></a>

```json
{
  "ExpenseId": 0,
  "BranchId": 0,
  "BranchName": "string",
  "CategoryId": 0,
  "CategoryName": "string",
  "UserId": 0,
  "UserName": "string",
  "Amount": 1000,
  "ExpenseDate": "2019-08-24T14:15:22Z",
  "Note": "string",
  "RefImage": "string",
  "PaymentMethod": "string",
  "Status": "string",
  "ApprovedBy": 0,
  "ApproverName": "string",
  "ApprovedAt": "2019-08-24T14:15:22Z",
  "RejectedBy": 0,
  "RejectorName": "string",
  "RejectedAt": "2019-08-24T14:15:22Z",
  "RejectionReason": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|ExpenseId|integer(int32)|false|none|none|
|BranchId|integer(int32)|true|none|none|
|BranchName|string|false|none|none|
|CategoryId|integer(int32)|true|none|none|
|CategoryName|string|false|none|none|
|UserId|integer(int32)|false|none|none|
|UserName|string|false|none|none|
|Amount|number(double)|true|none|none|
|ExpenseDate|string(date-time)|false|none|none|
|Note|string|false|none|none|
|RefImage|string|false|none|none|
|PaymentMethod|string|false|none|none|
|Status|string|false|none|none|
|ApprovedBy|integer(int32)|false|none|none|
|ApproverName|string|false|none|none|
|ApprovedAt|string(date-time)|false|none|none|
|RejectedBy|integer(int32)|false|none|none|
|RejectorName|string|false|none|none|
|RejectedAt|string(date-time)|false|none|none|
|RejectionReason|string|false|none|none|

<h2 id="tocS_UpdateExpenseStatusRequest">UpdateExpenseStatusRequest</h2>
<!-- backwards compatibility -->
<a id="schemaupdateexpensestatusrequest"></a>
<a id="schema_UpdateExpenseStatusRequest"></a>
<a id="tocSupdateexpensestatusrequest"></a>
<a id="tocsupdateexpensestatusrequest"></a>

```json
{
  "ExpenseId": 0,
  "Status": "string",
  "PaymentMethod": "string",
  "RejectReason": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|ExpenseId|integer(int32)|false|none|none|
|Status|string|false|none|none|
|PaymentMethod|string|false|none|none|
|RejectReason|string|false|none|none|

<h2 id="tocS_ExpenseCategoryDTO">ExpenseCategoryDTO</h2>
<!-- backwards compatibility -->
<a id="schemaexpensecategorydto"></a>
<a id="schema_ExpenseCategoryDTO"></a>
<a id="tocSexpensecategorydto"></a>
<a id="tocsexpensecategorydto"></a>

```json
{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "IsActive": true
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|CategoryId|integer(int32)|false|none|none|
|Name|string|false|none|none|
|Description|string|false|none|none|
|IsActive|boolean|false|none|none|

<h2 id="tocS_StockAdjustmentDTO">StockAdjustmentDTO</h2>
<!-- backwards compatibility -->
<a id="schemastockadjustmentdto"></a>
<a id="schema_StockAdjustmentDTO"></a>
<a id="tocSstockadjustmentdto"></a>
<a id="tocsstockadjustmentdto"></a>

```json
{
  "MovementType": "string",
  "WarehouseId": 0,
  "ProductId": 0,
  "QuantityChange": 0.1,
  "Reason": "string",
  "UserId": 0
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|MovementType|string|false|none|none|
|WarehouseId|integer(int32)|false|none|none|
|ProductId|integer(int32)|false|none|none|
|QuantityChange|number(double)|false|none|none|
|Reason|string|false|none|none|
|UserId|integer(int32)|false|none|none|

<h2 id="tocS_InventoryDTO">InventoryDTO</h2>
<!-- backwards compatibility -->
<a id="schemainventorydto"></a>
<a id="schema_InventoryDTO"></a>
<a id="tocSinventorydto"></a>
<a id="tocsinventorydto"></a>

```json
{
  "InventoryId": 0,
  "LotId": 0,
  "WarehouseId": 0,
  "Quantity": 0,
  "LastUpdate": "2019-08-24T14:15:22Z",
  "BranchName": "string",
  "WarehouseName": "string",
  "BatchCode": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|InventoryId|integer(int32)|false|none|none|
|LotId|integer(int32)|false|none|none|
|WarehouseId|integer(int32)|false|none|none|
|Quantity|integer(int32)|false|none|none|
|LastUpdate|string(date-time)|false|none|none|
|BranchName|string|false|none|none|
|WarehouseName|string|false|none|none|
|BatchCode|string|false|none|none|

<h2 id="tocS_PurchaseOrderDTO">PurchaseOrderDTO</h2>
<!-- backwards compatibility -->
<a id="schemapurchaseorderdto"></a>
<a id="schema_PurchaseOrderDTO"></a>
<a id="tocSpurchaseorderdto"></a>
<a id="tocspurchaseorderdto"></a>

```json
{
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "WarehouseId": 0,
  "UserId": 0,
  "BranchId": 0,
  "OrderDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "TotalAmount": 0.1,
  "PaymentStatus": "string",
  "Notes": "string",
  "Details": [
    {
      "PurchaseOrderDetailId": 0,
      "PurchaseOrderId": 0,
      "ProductId": 0,
      "CategoryId": 0,
      "LotId": 0,
      "Quantity": 0,
      "Unit": "string",
      "UnitPrice": 0.1,
      "TotalPrice": 0.1,
      "ProductName": "string",
      "CategoryName": "string",
      "ProductLots": [
        {
          "LotId": 0,
          "ProductId": 0,
          "ExpiryDate": "2019-08-24T14:15:22Z",
          "BatchCode": "string",
          "CostPrice": 0.1,
          "ManufacturingDate": "2019-08-24T14:15:22Z"
        }
      ]
    }
  ],
  "SupplierDetail": [
    {
      "DebtId": 0,
      "PurchaseOrderId": 0,
      "SupplierId": 0,
      "SupplierName": "string",
      "TotalAmount": 0.1,
      "PaidAmount": 0.1,
      "UnpaidAmount": 0.1,
      "DueDate": "2019-08-24T14:15:22Z",
      "Status": "string",
      "CreatedAt": "2019-08-24T14:15:22Z",
      "UpdatedAt": "2019-08-24T14:15:22Z",
      "SupplierEmail": "string"
    }
  ],
  "PaidAmount": 0.1,
  "DueAmount": 0.1,
  "SupplierName": "string",
  "WarehouseName": "string",
  "UserName": "string",
  "BranchName": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|PurchaseOrderId|integer(int32)|false|none|none|
|SupplierId|integer(int32)|false|none|none|
|WarehouseId|integer(int32)|false|none|none|
|UserId|integer(int32)|false|none|none|
|BranchId|integer(int32)|false|none|none|
|OrderDate|string(date-time)|false|none|none|
|Status|string|false|none|none|
|TotalAmount|number(double)|false|none|none|
|PaymentStatus|string|false|none|none|
|Notes|string|false|none|none|
|Details|[[PurchaseOrderDetailDTO](#schemapurchaseorderdetaildto)]|false|none|none|
|SupplierDetail|[[SupplierDebtDTO](#schemasupplierdebtdto)]|false|none|none|
|PaidAmount|number(double)|false|none|none|
|DueAmount|number(double)|false|none|none|
|SupplierName|string|false|none|none|
|WarehouseName|string|false|none|none|
|UserName|string|false|none|none|
|BranchName|string|false|none|none|

<h2 id="tocS_PurchaseOrderDetailDTO">PurchaseOrderDetailDTO</h2>
<!-- backwards compatibility -->
<a id="schemapurchaseorderdetaildto"></a>
<a id="schema_PurchaseOrderDetailDTO"></a>
<a id="tocSpurchaseorderdetaildto"></a>
<a id="tocspurchaseorderdetaildto"></a>

```json
{
  "PurchaseOrderDetailId": 0,
  "PurchaseOrderId": 0,
  "ProductId": 0,
  "CategoryId": 0,
  "LotId": 0,
  "Quantity": 0,
  "Unit": "string",
  "UnitPrice": 0.1,
  "TotalPrice": 0.1,
  "ProductName": "string",
  "CategoryName": "string",
  "ProductLots": [
    {
      "LotId": 0,
      "ProductId": 0,
      "ExpiryDate": "2019-08-24T14:15:22Z",
      "BatchCode": "string",
      "CostPrice": 0.1,
      "ManufacturingDate": "2019-08-24T14:15:22Z"
    }
  ]
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|PurchaseOrderDetailId|integer(int32)|false|none|none|
|PurchaseOrderId|integer(int32)|false|none|none|
|ProductId|integer(int32)|false|none|none|
|CategoryId|integer(int32)|false|none|none|
|LotId|integer(int32)|false|none|none|
|Quantity|integer(int32)|false|none|none|
|Unit|string|false|none|none|
|UnitPrice|number(double)|false|none|none|
|TotalPrice|number(double)|false|none|none|
|ProductName|string|false|none|none|
|CategoryName|string|false|none|none|
|ProductLots|[[ProductLotDTO](#schemaproductlotdto)]|false|none|none|

<h2 id="tocS_SupplierDebtDTO">SupplierDebtDTO</h2>
<!-- backwards compatibility -->
<a id="schemasupplierdebtdto"></a>
<a id="schema_SupplierDebtDTO"></a>
<a id="tocSsupplierdebtdto"></a>
<a id="tocssupplierdebtdto"></a>

```json
{
  "DebtId": 0,
  "PurchaseOrderId": 0,
  "SupplierId": 0,
  "SupplierName": "string",
  "TotalAmount": 0.1,
  "PaidAmount": 0.1,
  "UnpaidAmount": 0.1,
  "DueDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "SupplierEmail": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|DebtId|integer(int32)|false|none|none|
|PurchaseOrderId|integer(int32)|false|none|none|
|SupplierId|integer(int32)|false|none|none|
|SupplierName|string|false|none|none|
|TotalAmount|number(double)|false|none|none|
|PaidAmount|number(double)|false|none|none|
|UnpaidAmount|number(double)|false|none|none|
|DueDate|string(date-time)|false|none|none|
|Status|string|false|none|none|
|CreatedAt|string(date-time)|false|none|none|
|UpdatedAt|string(date-time)|false|none|none|
|SupplierEmail|string|false|none|none|

<h2 id="tocS_ProductLotDTO">ProductLotDTO</h2>
<!-- backwards compatibility -->
<a id="schemaproductlotdto"></a>
<a id="schema_ProductLotDTO"></a>
<a id="tocSproductlotdto"></a>
<a id="tocsproductlotdto"></a>

```json
{
  "LotId": 0,
  "ProductId": 0,
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "BatchCode": "string",
  "CostPrice": 0.1,
  "ManufacturingDate": "2019-08-24T14:15:22Z"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|LotId|integer(int32)|false|none|none|
|ProductId|integer(int32)|false|none|none|
|ExpiryDate|string(date-time)|false|none|none|
|BatchCode|string|false|none|none|
|CostPrice|number(double)|false|none|none|
|ManufacturingDate|string(date-time)|false|none|none|

<h2 id="tocS_SupplierPaymentDTO">SupplierPaymentDTO</h2>
<!-- backwards compatibility -->
<a id="schemasupplierpaymentdto"></a>
<a id="schema_SupplierPaymentDTO"></a>
<a id="tocSsupplierpaymentdto"></a>
<a id="tocssupplierpaymentdto"></a>

```json
{
  "PaymentId": 0,
  "UserId": 0,
  "DebtId": 0,
  "Amount": 0.1,
  "PaymentMethod": "string",
  "PaymentDate": "2019-08-24T14:15:22Z",
  "PaymentProof": "string",
  "SupplierName": "string",
  "SupplierEmail": "string",
  "UnpaidAmountAfterPayment": 0.1
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|PaymentId|integer(int32)|false|none|none|
|UserId|integer(int32)|false|none|none|
|DebtId|integer(int32)|false|none|none|
|Amount|number(double)|false|none|none|
|PaymentMethod|string|false|none|none|
|PaymentDate|string(date-time)|false|none|none|
|PaymentProof|string|false|none|none|
|SupplierName|string|false|none|none|
|SupplierEmail|string|false|none|none|
|UnpaidAmountAfterPayment|number(double)|false|none|none|

<h2 id="tocS_WarehouseDTO">WarehouseDTO</h2>
<!-- backwards compatibility -->
<a id="schemawarehousedto"></a>
<a id="schema_WarehouseDTO"></a>
<a id="tocSwarehousedto"></a>
<a id="tocswarehousedto"></a>

```json
{
  "WarehouseId": 0,
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "BranchName": "string",
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|WarehouseId|integer(int32)|false|none|none|
|BranchId|integer(int32)|false|none|none|
|Name|string|false|none|none|
|Address|string|false|none|none|
|AddressUrl|string|false|none|none|
|Latitude|number(double)|false|none|none|
|Longitude|number(double)|false|none|none|
|BranchName|string|false|none|none|
|CreatedAt|string(date-time)|false|none|none|
|UpdatedAt|string(date-time)|false|none|none|
|CanBeDeleted|boolean|false|none|none|

<h2 id="tocS_AuditLogFilter">AuditLogFilter</h2>
<!-- backwards compatibility -->
<a id="schemaauditlogfilter"></a>
<a id="schema_AuditLogFilter"></a>
<a id="tocSauditlogfilter"></a>
<a id="tocsauditlogfilter"></a>

```json
{
  "Start": 0,
  "Length": 0,
  "TableName": "string",
  "Action": "string",
  "Search": "string",
  "FromDate": "2019-08-24T14:15:22Z",
  "ToDate": "2019-08-24T14:15:22Z"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|Start|integer(int32)|false|none|none|
|Length|integer(int32)|false|none|none|
|TableName|string|false|none|none|
|Action|string|false|none|none|
|Search|string|false|none|none|
|FromDate|string(date-time)|false|none|none|
|ToDate|string(date-time)|false|none|none|

<h2 id="tocS_AppLogFilter">AppLogFilter</h2>
<!-- backwards compatibility -->
<a id="schemaapplogfilter"></a>
<a id="schema_AppLogFilter"></a>
<a id="tocSapplogfilter"></a>
<a id="tocsapplogfilter"></a>

```json
{
  "Start": 0,
  "Length": 0,
  "LogLevel": "string",
  "Logger": "string",
  "Search": "string",
  "FromDate": "2019-08-24T14:15:22Z",
  "ToDate": "2019-08-24T14:15:22Z"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|Start|integer(int32)|false|none|none|
|Length|integer(int32)|false|none|none|
|LogLevel|string|false|none|none|
|Logger|string|false|none|none|
|Search|string|false|none|none|
|FromDate|string(date-time)|false|none|none|
|ToDate|string(date-time)|false|none|none|

<h2 id="tocS_ProductDTO">ProductDTO</h2>
<!-- backwards compatibility -->
<a id="schemaproductdto"></a>
<a id="schema_ProductDTO"></a>
<a id="tocSproductdto"></a>
<a id="tocsproductdto"></a>

```json
{
  "ProductId": 0,
  "Name": "string",
  "CategoryId": 0,
  "SupplierId": 0,
  "Barcode": "string",
  "Price": 0.1,
  "Unit": "string",
  "Description": "string",
  "MinStock": 0,
  "MaxStock": 0,
  "Image": "string",
  "CategoryName": "string",
  "SupplierName": "string",
  "Quantity": 0,
  "ExpiredAt": "2019-08-24T14:15:22Z",
  "OriginalPrice": 0.1,
  "FinalPrice": 0.1,
  "PromotionName": "string",
  "DiscountAmount": 0.1
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|ProductId|integer(int32)|false|none|none|
|Name|string|false|none|none|
|CategoryId|integer(int32)|false|none|none|
|SupplierId|integer(int32)|false|none|none|
|Barcode|string|false|none|none|
|Price|number(double)|false|none|none|
|Unit|string|false|none|none|
|Description|string|false|none|none|
|MinStock|integer(int32)|false|none|none|
|MaxStock|integer(int32)|false|none|none|
|Image|string|false|none|none|
|CategoryName|string|false|none|none|
|SupplierName|string|false|none|none|
|Quantity|integer(int32)|false|none|none|
|ExpiredAt|string(date-time)|false|none|none|
|OriginalPrice|number(double)|false|none|none|
|FinalPrice|number(double)|false|none|none|
|PromotionName|string|false|none|none|
|DiscountAmount|number(double)|false|none|none|

<h2 id="tocS_ProductImageDTO">ProductImageDTO</h2>
<!-- backwards compatibility -->
<a id="schemaproductimagedto"></a>
<a id="schema_ProductImageDTO"></a>
<a id="tocSproductimagedto"></a>
<a id="tocsproductimagedto"></a>

```json
{
  "ImageId": 0,
  "ProductId": 0,
  "ImageUrl": "string",
  "SortOrder": 0,
  "CreatedAt": "2019-08-24T14:15:22Z"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|ImageId|integer(int32)|false|none|none|
|ProductId|integer(int32)|false|none|none|
|ImageUrl|string|false|none|none|
|SortOrder|integer(int32)|false|none|none|
|CreatedAt|string(date-time)|false|none|none|

<h2 id="tocS_ProductCategoryDTO">ProductCategoryDTO</h2>
<!-- backwards compatibility -->
<a id="schemaproductcategorydto"></a>
<a id="schema_ProductCategoryDTO"></a>
<a id="tocSproductcategorydto"></a>
<a id="tocsproductcategorydto"></a>

```json
{
  "CategoryId": 0,
  "Name": "string",
  "Description": "string",
  "CanBeDeleted": true
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|CategoryId|integer(int32)|false|none|none|
|Name|string|false|none|none|
|Description|string|false|none|none|
|CanBeDeleted|boolean|false|none|none|

<h2 id="tocS_SupplierDTO">SupplierDTO</h2>
<!-- backwards compatibility -->
<a id="schemasupplierdto"></a>
<a id="schema_SupplierDTO"></a>
<a id="tocSsupplierdto"></a>
<a id="tocssupplierdto"></a>

```json
{
  "SupplierId": 0,
  "Name": "string",
  "Email": "string",
  "Phone": "string",
  "Address": "string",
  "AddressUrl": "string",
  "ContactPerson": "string",
  "CanBeDeleted": true
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|SupplierId|integer(int32)|false|none|none|
|Name|string|false|none|none|
|Email|string|false|none|none|
|Phone|string|false|none|none|
|Address|string|false|none|none|
|AddressUrl|string|false|none|none|
|ContactPerson|string|false|none|none|
|CanBeDeleted|boolean|false|none|none|

<h2 id="tocS_QuotationDTO">QuotationDTO</h2>
<!-- backwards compatibility -->
<a id="schemaquotationdto"></a>
<a id="schema_QuotationDTO"></a>
<a id="tocSquotationdto"></a>
<a id="tocsquotationdto"></a>

```json
{
  "QuotationId": 0,
  "QuotationCode": "string",
  "BranchId": 0,
  "BranchName": "string",
  "CustomerId": 0,
  "CustomerName": "string",
  "CustomerPhone": "string",
  "CustomerAddress": "string",
  "UserId": 0,
  "CreatorName": "string",
  "IssueDate": "2019-08-24T14:15:22Z",
  "ExpiryDate": "2019-08-24T14:15:22Z",
  "TotalAmount": 0.1,
  "DiscountAmount": 0.1,
  "ManualDiscount": 0.1,
  "DiscountReason": "string",
  "FinalAmount": 0.1,
  "Status": "string",
  "Note": "string",
  "ConvertedOrderId": 0,
  "Details": [
    {
      "DetailId": 0,
      "ProductId": 0,
      "ProductName": "string",
      "ProductImage": "string",
      "Unit": "string",
      "Quantity": 0,
      "UnitPrice": 0.1,
      "Discount": 0.1,
      "TotalPrice": 0.1,
      "Note": "string"
    }
  ]
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|QuotationId|integer(int32)|false|none|none|
|QuotationCode|string|false|none|none|
|BranchId|integer(int32)|false|none|none|
|BranchName|string|false|none|none|
|CustomerId|integer(int32)|false|none|none|
|CustomerName|string|false|none|none|
|CustomerPhone|string|false|none|none|
|CustomerAddress|string|false|none|none|
|UserId|integer(int32)|false|none|none|
|CreatorName|string|false|none|none|
|IssueDate|string(date-time)|false|none|none|
|ExpiryDate|string(date-time)|false|none|none|
|TotalAmount|number(double)|false|none|none|
|DiscountAmount|number(double)|false|none|none|
|ManualDiscount|number(double)|false|none|none|
|DiscountReason|string|false|none|none|
|FinalAmount|number(double)|false|read-only|none|
|Status|string|false|none|none|
|Note|string|false|none|none|
|ConvertedOrderId|integer(int32)|false|none|none|
|Details|[[QuotationDetailDTO](#schemaquotationdetaildto)]|false|none|none|

<h2 id="tocS_QuotationDetailDTO">QuotationDetailDTO</h2>
<!-- backwards compatibility -->
<a id="schemaquotationdetaildto"></a>
<a id="schema_QuotationDetailDTO"></a>
<a id="tocSquotationdetaildto"></a>
<a id="tocsquotationdetaildto"></a>

```json
{
  "DetailId": 0,
  "ProductId": 0,
  "ProductName": "string",
  "ProductImage": "string",
  "Unit": "string",
  "Quantity": 0,
  "UnitPrice": 0.1,
  "Discount": 0.1,
  "TotalPrice": 0.1,
  "Note": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|DetailId|integer(int32)|false|none|none|
|ProductId|integer(int32)|false|none|none|
|ProductName|string|false|none|none|
|ProductImage|string|false|none|none|
|Unit|string|false|none|none|
|Quantity|integer(int32)|false|none|none|
|UnitPrice|number(double)|false|none|none|
|Discount|number(double)|false|none|none|
|TotalPrice|number(double)|false|none|none|
|Note|string|false|none|none|

<h2 id="tocS_ChangeStatusModel">ChangeStatusModel</h2>
<!-- backwards compatibility -->
<a id="schemachangestatusmodel"></a>
<a id="schema_ChangeStatusModel"></a>
<a id="tocSchangestatusmodel"></a>
<a id="tocschangestatusmodel"></a>

```json
{
  "Status": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|Status|string|false|none|none|

<h2 id="tocS_UpdateStatusModel">UpdateStatusModel</h2>
<!-- backwards compatibility -->
<a id="schemaupdatestatusmodel"></a>
<a id="schema_UpdateStatusModel"></a>
<a id="tocSupdatestatusmodel"></a>
<a id="tocsupdatestatusmodel"></a>

```json
{
  "Status": "string",
  "ConnectionId": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|Status|string|false|none|none|
|ConnectionId|string|false|none|none|

<h2 id="tocS_OrderDTO">OrderDTO</h2>
<!-- backwards compatibility -->
<a id="schemaorderdto"></a>
<a id="schema_OrderDTO"></a>
<a id="tocSorderdto"></a>
<a id="tocsorderdto"></a>

```json
{
  "OrderId": 0,
  "CustomerId": 0,
  "UserId": 0,
  "BranchId": 0,
  "OrderDate": "2019-08-24T14:15:22Z",
  "Status": "string",
  "TotalAmount": 0.1,
  "DeliveryAddressId": 0,
  "OrderDetails": [
    {
      "OrderDetailId": 0,
      "OrderId": 0,
      "ProductId": 0,
      "Quantity": 0,
      "UnitPrice": 0.1,
      "Discount": 0.1,
      "ProductName": "string",
      "CustomerId": 0,
      "QuantityReturned": 0
    }
  ],
  "PaymentMethod": "string",
  "PaymentStatus": "string",
  "CustomerName": "string",
  "UserName": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|OrderId|integer(int32)|false|none|none|
|CustomerId|integer(int32)|false|none|none|
|UserId|integer(int32)|false|none|none|
|BranchId|integer(int32)|false|none|none|
|OrderDate|string(date-time)|false|none|none|
|Status|string|false|none|none|
|TotalAmount|number(double)|false|none|none|
|DeliveryAddressId|integer(int32)|false|none|none|
|OrderDetails|[[OrderDetailDTO](#schemaorderdetaildto)]|false|none|none|
|PaymentMethod|string|false|none|none|
|PaymentStatus|string|false|none|none|
|CustomerName|string|false|none|none|
|UserName|string|false|none|none|

<h2 id="tocS_OrderDetailDTO">OrderDetailDTO</h2>
<!-- backwards compatibility -->
<a id="schemaorderdetaildto"></a>
<a id="schema_OrderDetailDTO"></a>
<a id="tocSorderdetaildto"></a>
<a id="tocsorderdetaildto"></a>

```json
{
  "OrderDetailId": 0,
  "OrderId": 0,
  "ProductId": 0,
  "Quantity": 0,
  "UnitPrice": 0.1,
  "Discount": 0.1,
  "ProductName": "string",
  "CustomerId": 0,
  "QuantityReturned": 0
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|OrderDetailId|integer(int32)|false|none|none|
|OrderId|integer(int32)|false|none|none|
|ProductId|integer(int32)|false|none|none|
|Quantity|integer(int32)|false|none|none|
|UnitPrice|number(double)|false|none|none|
|Discount|number(double)|false|none|none|
|ProductName|string|false|none|none|
|CustomerId|integer(int32)|false|none|none|
|QuantityReturned|integer(int32)|false|none|none|

<h2 id="tocS_CheckoutRequestDTO">CheckoutRequestDTO</h2>
<!-- backwards compatibility -->
<a id="schemacheckoutrequestdto"></a>
<a id="schema_CheckoutRequestDTO"></a>
<a id="tocScheckoutrequestdto"></a>
<a id="tocscheckoutrequestdto"></a>

```json
{
  "UserId": 0,
  "BranchId": 0,
  "CustomerId": 0,
  "PointsUsed": 0,
  "PointsEarned": 0,
  "TotalAmount": 0.1,
  "ManualDiscount": 0.1,
  "ManualDiscountReason": "string",
  "ParentOrderId": 0,
  "Status": "string",
  "PaymentMethod": "string",
  "ConnectionId": "string",
  "Items": [
    {
      "ProductId": 0,
      "Quantity": 0,
      "Price": 0.1,
      "Discount": 0.1
    }
  ]
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|UserId|integer(int32)|false|none|none|
|BranchId|integer(int32)|false|none|none|
|CustomerId|integer(int32)|false|none|none|
|PointsUsed|integer(int32)|false|none|none|
|PointsEarned|integer(int32)|false|none|none|
|TotalAmount|number(double)|false|none|none|
|ManualDiscount|number(double)|false|none|none|
|ManualDiscountReason|string|false|none|none|
|ParentOrderId|integer(int32)|false|none|none|
|Status|string|false|none|none|
|PaymentMethod|string|false|none|none|
|ConnectionId|string|false|none|none|
|Items|[[CartItemDTO](#schemacartitemdto)]|true|none|none|

<h2 id="tocS_CartItemDTO">CartItemDTO</h2>
<!-- backwards compatibility -->
<a id="schemacartitemdto"></a>
<a id="schema_CartItemDTO"></a>
<a id="tocScartitemdto"></a>
<a id="tocscartitemdto"></a>

```json
{
  "ProductId": 0,
  "Quantity": 0,
  "Price": 0.1,
  "Discount": 0.1
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|ProductId|integer(int32)|false|none|none|
|Quantity|integer(int32)|false|none|none|
|Price|number(double)|false|none|none|
|Discount|number(double)|false|none|none|

<h2 id="tocS_CreateQrRequestDTO">CreateQrRequestDTO</h2>
<!-- backwards compatibility -->
<a id="schemacreateqrrequestdto"></a>
<a id="schema_CreateQrRequestDTO"></a>
<a id="tocScreateqrrequestdto"></a>
<a id="tocscreateqrrequestdto"></a>

```json
{
  "Amount": 0,
  "Description": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|Amount|integer(int32)|false|none|none|
|Description|string|false|none|none|

<h2 id="tocS_PromotionDTO">PromotionDTO</h2>
<!-- backwards compatibility -->
<a id="schemapromotiondto"></a>
<a id="schema_PromotionDTO"></a>
<a id="tocSpromotiondto"></a>
<a id="tocspromotiondto"></a>

```json
{
  "PromotionId": 0,
  "Name": "string",
  "DiscountType": "string",
  "DiscountValue": 0.1,
  "CategoryId": 0,
  "CustomerType": "string",
  "StartDate": "2019-08-24T14:15:22Z",
  "EndDate": "2019-08-24T14:15:22Z",
  "Priority": 0,
  "IsActive": true,
  "CategoryName": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|PromotionId|integer(int32)|false|none|none|
|Name|string|false|none|none|
|DiscountType|string|false|none|none|
|DiscountValue|number(double)|false|none|none|
|CategoryId|integer(int32)|false|none|none|
|CustomerType|string|false|none|none|
|StartDate|string(date-time)|false|none|none|
|EndDate|string(date-time)|false|none|none|
|Priority|integer(int32)|false|none|none|
|IsActive|boolean|false|none|none|
|CategoryName|string|false|none|none|

<h2 id="tocS_LoginRequestDTO">LoginRequestDTO</h2>
<!-- backwards compatibility -->
<a id="schemaloginrequestdto"></a>
<a id="schema_LoginRequestDTO"></a>
<a id="tocSloginrequestdto"></a>
<a id="tocsloginrequestdto"></a>

```json
{
  "EmailOrUsername": "string",
  "Password": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|EmailOrUsername|string|false|none|none|
|Password|string|false|none|none|

<h2 id="tocS_ExternalLoginRequestDTO">ExternalLoginRequestDTO</h2>
<!-- backwards compatibility -->
<a id="schemaexternalloginrequestdto"></a>
<a id="schema_ExternalLoginRequestDTO"></a>
<a id="tocSexternalloginrequestdto"></a>
<a id="tocsexternalloginrequestdto"></a>

```json
{
  "Provider": "string",
  "Email": "string",
  "IdToken": "string",
  "FullName": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|Provider|string|false|none|none|
|Email|string|false|none|none|
|IdToken|string|false|none|none|
|FullName|string|false|none|none|

<h2 id="tocS_RequestOtpModel">RequestOtpModel</h2>
<!-- backwards compatibility -->
<a id="schemarequestotpmodel"></a>
<a id="schema_RequestOtpModel"></a>
<a id="tocSrequestotpmodel"></a>
<a id="tocsrequestotpmodel"></a>

```json
{
  "Email": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|Email|string|false|none|none|

<h2 id="tocS_ResetPasswordRequestDTO">ResetPasswordRequestDTO</h2>
<!-- backwards compatibility -->
<a id="schemaresetpasswordrequestdto"></a>
<a id="schema_ResetPasswordRequestDTO"></a>
<a id="tocSresetpasswordrequestdto"></a>
<a id="tocsresetpasswordrequestdto"></a>

```json
{
  "Email": "string",
  "Otp": "string",
  "NewPassword": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|Email|string|false|none|none|
|Otp|string|false|none|none|
|NewPassword|string|false|none|none|

<h2 id="tocS_CurrentUserDTO">CurrentUserDTO</h2>
<!-- backwards compatibility -->
<a id="schemacurrentuserdto"></a>
<a id="schema_CurrentUserDTO"></a>
<a id="tocScurrentuserdto"></a>
<a id="tocscurrentuserdto"></a>

```json
{
  "UserId": 0,
  "Username": "string",
  "FullName": "string",
  "Email": "string",
  "Image": "string",
  "Phone": "string",
  "Status": "string",
  "SupplierId": 0,
  "BranchId": 0,
  "RoleId": 0,
  "Roles": [
    {
      "RoleId": 0,
      "Name": "string",
      "Permissions": [
        {
          "PermissionId": 0,
          "Name": "string",
          "Module": "string"
        }
      ]
    }
  ],
  "Permissions": [
    {
      "PermissionId": 0,
      "Name": "string",
      "Module": "string"
    }
  ],
  "IsAdmin": true,
  "IsSupplier": true
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|UserId|integer(int32)|false|none|none|
|Username|string|false|none|none|
|FullName|string|false|none|none|
|Email|string|false|none|none|
|Image|string|false|none|none|
|Phone|string|false|none|none|
|Status|string|false|none|none|
|SupplierId|integer(int32)|false|none|none|
|BranchId|integer(int32)|false|none|none|
|RoleId|integer(int32)|false|none|none|
|Roles|[[RoleDTO](#schemaroledto)]|false|none|none|
|Permissions|[[PermissionDTO](#schemapermissiondto)]|false|none|none|
|IsAdmin|boolean|false|none|none|
|IsSupplier|boolean|false|none|none|

<h2 id="tocS_RoleDTO">RoleDTO</h2>
<!-- backwards compatibility -->
<a id="schemaroledto"></a>
<a id="schema_RoleDTO"></a>
<a id="tocSroledto"></a>
<a id="tocsroledto"></a>

```json
{
  "RoleId": 0,
  "Name": "string",
  "Permissions": [
    {
      "PermissionId": 0,
      "Name": "string",
      "Module": "string"
    }
  ]
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|RoleId|integer(int32)|false|none|none|
|Name|string|false|none|none|
|Permissions|[[PermissionDTO](#schemapermissiondto)]|false|none|none|

<h2 id="tocS_PermissionDTO">PermissionDTO</h2>
<!-- backwards compatibility -->
<a id="schemapermissiondto"></a>
<a id="schema_PermissionDTO"></a>
<a id="tocSpermissiondto"></a>
<a id="tocspermissiondto"></a>

```json
{
  "PermissionId": 0,
  "Name": "string",
  "Module": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|PermissionId|integer(int32)|false|none|none|
|Name|string|false|none|none|
|Module|string|false|none|none|

<h2 id="tocS_UpdateEmailModel">UpdateEmailModel</h2>
<!-- backwards compatibility -->
<a id="schemaupdateemailmodel"></a>
<a id="schema_UpdateEmailModel"></a>
<a id="tocSupdateemailmodel"></a>
<a id="tocsupdateemailmodel"></a>

```json
{
  "UserId": 0,
  "NewEmail": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|UserId|integer(int32)|false|none|none|
|NewEmail|string|false|none|none|

<h2 id="tocS_VerifyChangeEmailModel">VerifyChangeEmailModel</h2>
<!-- backwards compatibility -->
<a id="schemaverifychangeemailmodel"></a>
<a id="schema_VerifyChangeEmailModel"></a>
<a id="tocSverifychangeemailmodel"></a>
<a id="tocsverifychangeemailmodel"></a>

```json
{
  "UserId": 0,
  "CurrentEmail": "string",
  "Otp": "string",
  "NewEmail": "string"
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|UserId|integer(int32)|false|none|none|
|CurrentEmail|string|false|none|none|
|Otp|string|false|none|none|
|NewEmail|string|false|none|none|

<h2 id="tocS_BranchDTO">BranchDTO</h2>
<!-- backwards compatibility -->
<a id="schemabranchdto"></a>
<a id="schema_BranchDTO"></a>
<a id="tocSbranchdto"></a>
<a id="tocsbranchdto"></a>

```json
{
  "BranchId": 0,
  "Name": "string",
  "Address": "string",
  "AddressUrl": "string",
  "Latitude": 0.1,
  "Longitude": 0.1,
  "CreatedAt": "2019-08-24T14:15:22Z",
  "UpdatedAt": "2019-08-24T14:15:22Z",
  "CanBeDeleted": true,
  "Distance": 0.1
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|BranchId|integer(int32)|false|none|none|
|Name|string|false|none|none|
|Address|string|false|none|none|
|AddressUrl|string|false|none|none|
|Latitude|number(double)|false|none|none|
|Longitude|number(double)|false|none|none|
|CreatedAt|string(date-time)|false|none|none|
|UpdatedAt|string(date-time)|false|none|none|
|CanBeDeleted|boolean|false|none|none|
|Distance|number(double)|false|none|none|

<h2 id="tocS_RolePermissionUpdateDTO">RolePermissionUpdateDTO</h2>
<!-- backwards compatibility -->
<a id="schemarolepermissionupdatedto"></a>
<a id="schema_RolePermissionUpdateDTO"></a>
<a id="tocSrolepermissionupdatedto"></a>
<a id="tocsrolepermissionupdatedto"></a>

```json
{
  "RoleId": 0,
  "PermissionIds": [
    0
  ]
}

```

### Properties

|Name|Type|Required|Restrictions|Description|
|---|---|---|---|---|
|RoleId|integer(int32)|false|none|none|
|PermissionIds|[integer]|false|none|none|

