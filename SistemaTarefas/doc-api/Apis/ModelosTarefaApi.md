# ModelosTarefaApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiModelosTarefaGet**](ModelosTarefaApi.md#apiModelosTarefaGet) | **GET** /api/ModelosTarefa |  |
| [**apiModelosTarefaIdModeloTarefaDelete**](ModelosTarefaApi.md#apiModelosTarefaIdModeloTarefaDelete) | **DELETE** /api/ModelosTarefa/{idModeloTarefa} |  |
| [**apiModelosTarefaIdModeloTarefaGet**](ModelosTarefaApi.md#apiModelosTarefaIdModeloTarefaGet) | **GET** /api/ModelosTarefa/{idModeloTarefa} |  |
| [**apiModelosTarefaIdModeloTarefaPut**](ModelosTarefaApi.md#apiModelosTarefaIdModeloTarefaPut) | **PUT** /api/ModelosTarefa/{idModeloTarefa} |  |
| [**apiModelosTarefaPost**](ModelosTarefaApi.md#apiModelosTarefaPost) | **POST** /api/ModelosTarefa |  |


<a name="apiModelosTarefaGet"></a>
# **apiModelosTarefaGet**
> List apiModelosTarefaGet(nomeModeloTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **nomeModeloTarefa** | **String**|  | [optional] [default to null] |

### Return type

[**List**](../Models/ModeloTarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiModelosTarefaIdModeloTarefaDelete"></a>
# **apiModelosTarefaIdModeloTarefaDelete**
> ResponseModel apiModelosTarefaIdModeloTarefaDelete(idModeloTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idModeloTarefa** | **Integer**|  | [default to null] |

### Return type

[**ResponseModel**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiModelosTarefaIdModeloTarefaGet"></a>
# **apiModelosTarefaIdModeloTarefaGet**
> ModeloTarefaResponse apiModelosTarefaIdModeloTarefaGet(idModeloTarefa, nomeModeloTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idModeloTarefa** | **Integer**|  | [default to null] |
| **nomeModeloTarefa** | **String**|  | [optional] [default to null] |

### Return type

[**ModeloTarefaResponse**](../Models/ModeloTarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiModelosTarefaIdModeloTarefaPut"></a>
# **apiModelosTarefaIdModeloTarefaPut**
> ModeloTarefaResponse apiModelosTarefaIdModeloTarefaPut(idModeloTarefa, ModeloTarefaRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idModeloTarefa** | **Integer**|  | [default to null] |
| **ModeloTarefaRequest** | [**ModeloTarefaRequest**](../Models/ModeloTarefaRequest.md)|  | [optional] |

### Return type

[**ModeloTarefaResponse**](../Models/ModeloTarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

<a name="apiModelosTarefaPost"></a>
# **apiModelosTarefaPost**
> ModeloTarefaResponse apiModelosTarefaPost(ModeloTarefaRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **ModeloTarefaRequest** | [**ModeloTarefaRequest**](../Models/ModeloTarefaRequest.md)|  | [optional] |

### Return type

[**ModeloTarefaResponse**](../Models/ModeloTarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

