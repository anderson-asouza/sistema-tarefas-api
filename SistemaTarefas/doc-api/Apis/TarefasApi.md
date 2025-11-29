# TarefasApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiTarefasAtivarDesativarIdTarefaPost**](TarefasApi.md#apiTarefasAtivarDesativarIdTarefaPost) | **POST** /api/Tarefas/ativar-desativar/{idTarefa} |  |
| [**apiTarefasGet**](TarefasApi.md#apiTarefasGet) | **GET** /api/Tarefas |  |
| [**apiTarefasIdTarefaDelete**](TarefasApi.md#apiTarefasIdTarefaDelete) | **DELETE** /api/Tarefas/{idTarefa} |  |
| [**apiTarefasIdTarefaGet**](TarefasApi.md#apiTarefasIdTarefaGet) | **GET** /api/Tarefas/{idTarefa} |  |
| [**apiTarefasIdTarefaPut**](TarefasApi.md#apiTarefasIdTarefaPut) | **PUT** /api/Tarefas/{idTarefa} |  |
| [**apiTarefasMarcarFlagIdTarefaPost**](TarefasApi.md#apiTarefasMarcarFlagIdTarefaPost) | **POST** /api/Tarefas/marcar-flag/{idTarefa} |  |
| [**apiTarefasPost**](TarefasApi.md#apiTarefasPost) | **POST** /api/Tarefas |  |


<a name="apiTarefasAtivarDesativarIdTarefaPost"></a>
# **apiTarefasAtivarDesativarIdTarefaPost**
> TarefaResponse apiTarefasAtivarDesativarIdTarefaPost(idTarefa, ativar)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTarefa** | **Integer**|  | [default to null] |
| **ativar** | **Boolean**|  | [optional] [default to false] |

### Return type

[**TarefaResponse**](../Models/TarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTarefasGet"></a>
# **apiTarefasGet**
> List apiTarefasGet(statusBusca, nomeTarefa, ordenarPelaDataInicial)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **statusBusca** | **String**|  | [optional] [default to null] |
| **nomeTarefa** | **String**|  | [optional] [default to null] |
| **ordenarPelaDataInicial** | **Boolean**|  | [optional] [default to false] |

### Return type

[**List**](../Models/TarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTarefasIdTarefaDelete"></a>
# **apiTarefasIdTarefaDelete**
> ResponseModel apiTarefasIdTarefaDelete(idTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTarefa** | **Integer**|  | [default to null] |

### Return type

[**ResponseModel**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTarefasIdTarefaGet"></a>
# **apiTarefasIdTarefaGet**
> TarefaResponse apiTarefasIdTarefaGet(idTarefa, nomeTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTarefa** | **Integer**|  | [default to null] |
| **nomeTarefa** | **String**|  | [optional] [default to null] |

### Return type

[**TarefaResponse**](../Models/TarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTarefasIdTarefaPut"></a>
# **apiTarefasIdTarefaPut**
> TarefaResponse apiTarefasIdTarefaPut(idTarefa, TarefaUpdRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTarefa** | **Integer**|  | [default to null] |
| **TarefaUpdRequest** | [**TarefaUpdRequest**](../Models/TarefaUpdRequest.md)|  | [optional] |

### Return type

[**TarefaResponse**](../Models/TarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

<a name="apiTarefasMarcarFlagIdTarefaPost"></a>
# **apiTarefasMarcarFlagIdTarefaPost**
> TarefaResponse apiTarefasMarcarFlagIdTarefaPost(idTarefa, idFlag)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTarefa** | **Integer**|  | [default to null] |
| **idFlag** | **Integer**|  | [optional] [default to 0] |

### Return type

[**TarefaResponse**](../Models/TarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTarefasPost"></a>
# **apiTarefasPost**
> TarefaResponse apiTarefasPost(TarefaRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **TarefaRequest** | [**TarefaRequest**](../Models/TarefaRequest.md)|  | [optional] |

### Return type

[**TarefaResponse**](../Models/TarefaResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

