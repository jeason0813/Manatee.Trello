---
title: IRestClient
category: API
order: 161
---

Defines methods required to make RESTful calls.

**Assembly:** Manatee.Trello.dll

**Namespace:** Manatee.Trello.Rest

**Inheritance hierarchy:**

- IRestClient

## Methods

### Task&lt;[IRestResponse](../IRestResponse#irestresponse)&gt; Execute(IRestRequest request, CancellationToken ct)

Makes a RESTful call and ignores any return data.

**Parameter:** request

The request.

**Parameter:** ct

(Optional) A cancellation token for async processing.

### Task&lt;[IRestResponse`1](../IRestResponse`1#irestresponse1)&gt; Execute&lt;T&gt;(IRestRequest request, CancellationToken ct)

Makes a RESTful call and expects a single object to be returned.

**Type Parameter:** T (no constraints)

The expected type of object to receive in response.

**Parameter:** request

The request.

**Parameter:** ct

(Optional) A cancellation token for async processing.

**Returns:** The response.
