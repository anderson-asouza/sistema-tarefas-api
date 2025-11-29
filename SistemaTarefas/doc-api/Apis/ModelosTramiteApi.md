# ModelosTramiteApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiModelosTramiteGet**](ModelosTramiteApi.md#apiModelosTramiteGet) | **GET** /api/ModelosTramite |  |
| [**apiModelosTramiteIdModeloTramiteDelete**](ModelosTramiteApi.md#apiModelosTramiteIdModeloTramiteDelete) | **DELETE** /api/ModelosTramite/{idModeloTramite} |  |
| [**apiModelosTramiteIdModeloTramiteGet**](ModelosTramiteApi.md#apiModelosTramiteIdModeloTramiteGet) | **GET** /api/ModelosTramite/{idModeloTramite} |  |
| [**apiModelosTramiteIdModeloTramitePut**](ModelosTramiteApi.md#apiModelosTramiteIdModeloTramitePut) | **PUT** /api/ModelosTramite/{idModeloTramite} |  |
| [**apiModelosTramitePost**](ModelosTramiteApi.md#apiModelosTramitePost) | **POST** /api/ModelosTramite |  |


<a name="apiModelosTramiteGet"></a>
# **apiModelosTramiteGet**
> List apiModelosTramiteGet(idModeloTarefa, nomeModeloTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idModeloTarefa** | **Integer**|  | [optional] [default to 0] |
| **nomeModeloTarefa** | **String**|  | [optional] [default to null] |

### Return type

[**List**](../Models/ModeloTramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiModelosTramiteIdModeloTramiteDelete"></a>
# **apiModelosTramiteIdModeloTramiteDelete**
> ResponseModel apiModelosTramiteIdModeloTramiteDelete(idModeloTramite)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idModeloTramite** | **Integer**|  | [default to null] |

### Return type

[**ResponseModel**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiModelosTramiteIdModeloTramiteGet"></a>
# **apiModelosTramiteIdModeloTramiteGet**
> ModeloTramiteResponse apiModelosTramiteIdModeloTramiteGet(idModeloTramite)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idModeloTramite** | **Integer**|  | [default to null] |

### Return type

[**ModeloTramiteResponse**](../Models/ModeloTramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiModelosTramiteIdModeloTramitePut"></a>
# **apiModelosTramiteIdModeloTramitePut**
> ModeloTramiteResponse apiModelosTramiteIdModeloTramitePut(idModeloTramite, ModeloTramiteUpdRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idModeloTramite** | **Integer**|  | [default to null] |
| **ModeloTramiteUpdRequest** | [**ModeloTramiteUpdRequest**](../Models/ModeloTramiteUpdRequest.md)|  | [optional] |

### Return type

[**ModeloTramiteResponse**](../Models/ModeloTramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

<a name="apiModelosTramitePost"></a>
# **apiModelosTramitePost**
> ModeloTramiteResponse apiModelosTramitePost(ModeloTramiteRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **ModeloTramiteRequest** | [**ModeloTramiteRequest**](../Models/ModeloTramiteRequest.md)|  | [optional] |

### Return type

[**ModeloTramiteResponse**](../Models/ModeloTramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

