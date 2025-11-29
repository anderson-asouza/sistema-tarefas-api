# TramitesApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiTramitesAssumirTramiteIdTramitePost**](TramitesApi.md#apiTramitesAssumirTramiteIdTramitePost) | **POST** /api/Tramites/assumir-tramite/{idTramite} |  |
| [**apiTramitesBuscarTramitesResponsavelGet**](TramitesApi.md#apiTramitesBuscarTramitesResponsavelGet) | **GET** /api/Tramites/buscar-tramites-responsavel |  |
| [**apiTramitesBuscarTramitesRevisorGet**](TramitesApi.md#apiTramitesBuscarTramitesRevisorGet) | **GET** /api/Tramites/buscar-tramites-revisor |  |
| [**apiTramitesBuscarTramitesTramitadorGet**](TramitesApi.md#apiTramitesBuscarTramitesTramitadorGet) | **GET** /api/Tramites/buscar-tramites-tramitador |  |
| [**apiTramitesBuscarTramitesUsuarioGet**](TramitesApi.md#apiTramitesBuscarTramitesUsuarioGet) | **GET** /api/Tramites/buscar-tramites-usuario |  |
| [**apiTramitesComecarExecucaoTramiteIdTramitePost**](TramitesApi.md#apiTramitesComecarExecucaoTramiteIdTramitePost) | **POST** /api/Tramites/comecar-execucao-tramite/{idTramite} |  |
| [**apiTramitesFinalizarExecucaoTramitePost**](TramitesApi.md#apiTramitesFinalizarExecucaoTramitePost) | **POST** /api/Tramites/finalizar-execucao-tramite |  |
| [**apiTramitesGet**](TramitesApi.md#apiTramitesGet) | **GET** /api/Tramites |  |
| [**apiTramitesIdTramiteGet**](TramitesApi.md#apiTramitesIdTramiteGet) | **GET** /api/Tramites/{idTramite} |  |
| [**apiTramitesIncluirTramiteIdTarefaPost**](TramitesApi.md#apiTramitesIncluirTramiteIdTarefaPost) | **POST** /api/Tramites/incluir-tramite/{idTarefa} |  |
| [**apiTramitesRetrocederTramiteIdTramitePost**](TramitesApi.md#apiTramitesRetrocederTramiteIdTramitePost) | **POST** /api/Tramites/retroceder-tramite/{idTramite} |  |
| [**apiTramitesRevisarTramiteAprovadoPost**](TramitesApi.md#apiTramitesRevisarTramiteAprovadoPost) | **POST** /api/Tramites/revisar-tramite/{aprovado} |  |


<a name="apiTramitesAssumirTramiteIdTramitePost"></a>
# **apiTramitesAssumirTramiteIdTramitePost**
> ResponseModel apiTramitesAssumirTramiteIdTramitePost(idTramite)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTramite** | **Integer**|  | [default to null] |

### Return type

[**ResponseModel**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesBuscarTramitesResponsavelGet"></a>
# **apiTramitesBuscarTramitesResponsavelGet**
> List apiTramitesBuscarTramitesResponsavelGet(ordenarDataComeco, idUsuario, statusTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **ordenarDataComeco** | **Boolean**|  | [optional] [default to false] |
| **idUsuario** | **Integer**|  | [optional] [default to 0] |
| **statusTarefa** | **String**|  | [optional] [default to A] |

### Return type

[**List**](../Models/TramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesBuscarTramitesRevisorGet"></a>
# **apiTramitesBuscarTramitesRevisorGet**
> List apiTramitesBuscarTramitesRevisorGet(ordenarDataComeco, idUsuario, statusTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **ordenarDataComeco** | **Boolean**|  | [optional] [default to false] |
| **idUsuario** | **Integer**|  | [optional] [default to 0] |
| **statusTarefa** | **String**|  | [optional] [default to A] |

### Return type

[**List**](../Models/TramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesBuscarTramitesTramitadorGet"></a>
# **apiTramitesBuscarTramitesTramitadorGet**
> List apiTramitesBuscarTramitesTramitadorGet(ordenarDataComeco, idUsuario, statusTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **ordenarDataComeco** | **Boolean**|  | [optional] [default to false] |
| **idUsuario** | **Integer**|  | [optional] [default to 0] |
| **statusTarefa** | **String**|  | [optional] [default to A] |

### Return type

[**List**](../Models/TramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesBuscarTramitesUsuarioGet"></a>
# **apiTramitesBuscarTramitesUsuarioGet**
> List apiTramitesBuscarTramitesUsuarioGet(ordenarDataComeco, idUsuario, statusTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **ordenarDataComeco** | **Boolean**|  | [optional] [default to false] |
| **idUsuario** | **Integer**|  | [optional] [default to 0] |
| **statusTarefa** | **String**|  | [optional] [default to A] |

### Return type

[**List**](../Models/TramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesComecarExecucaoTramiteIdTramitePost"></a>
# **apiTramitesComecarExecucaoTramiteIdTramitePost**
> ResponseModel apiTramitesComecarExecucaoTramiteIdTramitePost(idTramite)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTramite** | **Integer**|  | [default to null] |

### Return type

[**ResponseModel**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesFinalizarExecucaoTramitePost"></a>
# **apiTramitesFinalizarExecucaoTramitePost**
> ResponseModel apiTramitesFinalizarExecucaoTramitePost(TramiteNotaRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **TramiteNotaRequest** | [**TramiteNotaRequest**](../Models/TramiteNotaRequest.md)|  | [optional] |

### Return type

[**ResponseModel**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesGet"></a>
# **apiTramitesGet**
> List apiTramitesGet(idTarefa, statusTramite, statusTarefa, ordenarPelaDataComecoTarefa)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTarefa** | **Integer**|  | [optional] [default to 0] |
| **statusTramite** | **Integer**|  | [optional] [default to 0] |
| **statusTarefa** | **String**|  | [optional] [default to null] |
| **ordenarPelaDataComecoTarefa** | **Boolean**|  | [optional] [default to false] |

### Return type

[**List**](../Models/TramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesIdTramiteGet"></a>
# **apiTramitesIdTramiteGet**
> TramiteResponse apiTramitesIdTramiteGet(idTramite)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTramite** | **Integer**|  | [default to null] |

### Return type

[**TramiteResponse**](../Models/TramiteResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesIncluirTramiteIdTarefaPost"></a>
# **apiTramitesIncluirTramiteIdTarefaPost**
> ResponseModel apiTramitesIncluirTramiteIdTarefaPost(idTarefa)



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

<a name="apiTramitesRetrocederTramiteIdTramitePost"></a>
# **apiTramitesRetrocederTramiteIdTramitePost**
> ResponseModel apiTramitesRetrocederTramiteIdTramitePost(idTramite)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idTramite** | **Integer**|  | [default to null] |

### Return type

[**ResponseModel**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiTramitesRevisarTramiteAprovadoPost"></a>
# **apiTramitesRevisarTramiteAprovadoPost**
> ResponseModel apiTramitesRevisarTramiteAprovadoPost(aprovado, TramiteNotaRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **aprovado** | **Boolean**|  | [default to null] |
| **TramiteNotaRequest** | [**TramiteNotaRequest**](../Models/TramiteNotaRequest.md)|  | [optional] |

### Return type

[**ResponseModel**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

