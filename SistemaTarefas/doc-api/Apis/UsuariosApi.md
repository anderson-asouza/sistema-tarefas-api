# UsuariosApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiUsuariosAlterarSenhaPost**](UsuariosApi.md#apiUsuariosAlterarSenhaPost) | **POST** /api/Usuarios/alterar-senha |  |
| [**apiUsuariosGet**](UsuariosApi.md#apiUsuariosGet) | **GET** /api/Usuarios |  |
| [**apiUsuariosIdUsuarioDelete**](UsuariosApi.md#apiUsuariosIdUsuarioDelete) | **DELETE** /api/Usuarios/{idUsuario} |  |
| [**apiUsuariosIdUsuarioGet**](UsuariosApi.md#apiUsuariosIdUsuarioGet) | **GET** /api/Usuarios/{idUsuario} |  |
| [**apiUsuariosIdUsuarioPut**](UsuariosApi.md#apiUsuariosIdUsuarioPut) | **PUT** /api/Usuarios/{idUsuario} |  |
| [**apiUsuariosLogarPost**](UsuariosApi.md#apiUsuariosLogarPost) | **POST** /api/Usuarios/logar |  |
| [**apiUsuariosPost**](UsuariosApi.md#apiUsuariosPost) | **POST** /api/Usuarios |  |
| [**apiUsuariosRemoverImagemPerfilDelete**](UsuariosApi.md#apiUsuariosRemoverImagemPerfilDelete) | **DELETE** /api/Usuarios/remover-imagem-perfil |  |
| [**apiUsuariosUploadImagemPerfilPost**](UsuariosApi.md#apiUsuariosUploadImagemPerfilPost) | **POST** /api/Usuarios/upload-imagem-perfil |  |


<a name="apiUsuariosAlterarSenhaPost"></a>
# **apiUsuariosAlterarSenhaPost**
> List apiUsuariosAlterarSenhaPost(trocarSenhaPeloAdm, UsuarioAlterarSenhaRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **trocarSenhaPeloAdm** | **Boolean**|  | [optional] [default to false] |
| **UsuarioAlterarSenhaRequest** | [**UsuarioAlterarSenhaRequest**](../Models/UsuarioAlterarSenhaRequest.md)|  | [optional] |

### Return type

[**List**](../Models/UsuarioLoginResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

<a name="apiUsuariosGet"></a>
# **apiUsuariosGet**
> List apiUsuariosGet(nomeUsuario)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **nomeUsuario** | **String**|  | [optional] [default to null] |

### Return type

[**List**](../Models/UsuarioResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiUsuariosIdUsuarioDelete"></a>
# **apiUsuariosIdUsuarioDelete**
> List apiUsuariosIdUsuarioDelete(idUsuario)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idUsuario** | **Integer**|  | [default to null] |

### Return type

[**List**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiUsuariosIdUsuarioGet"></a>
# **apiUsuariosIdUsuarioGet**
> List apiUsuariosIdUsuarioGet(idUsuario, nomeUsuario, matriculaUsuario, login)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idUsuario** | **Integer**|  | [default to null] |
| **nomeUsuario** | **String**|  | [optional] [default to ] |
| **matriculaUsuario** | **String**|  | [optional] [default to ] |
| **login** | **String**|  | [optional] [default to ] |

### Return type

[**List**](../Models/UsuarioResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiUsuariosIdUsuarioPut"></a>
# **apiUsuariosIdUsuarioPut**
> List apiUsuariosIdUsuarioPut(idUsuario, UsuarioUpdRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **idUsuario** | **Integer**|  | [default to null] |
| **UsuarioUpdRequest** | [**UsuarioUpdRequest**](../Models/UsuarioUpdRequest.md)|  | [optional] |

### Return type

[**List**](../Models/UsuarioResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

<a name="apiUsuariosLogarPost"></a>
# **apiUsuariosLogarPost**
> List apiUsuariosLogarPost(UsuarioLoginRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **UsuarioLoginRequest** | [**UsuarioLoginRequest**](../Models/UsuarioLoginRequest.md)|  | [optional] |

### Return type

[**List**](../Models/UsuarioLoginResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

<a name="apiUsuariosPost"></a>
# **apiUsuariosPost**
> List apiUsuariosPost(UsuarioRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **UsuarioRequest** | [**UsuarioRequest**](../Models/UsuarioRequest.md)|  | [optional] |

### Return type

[**List**](../Models/UsuarioResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

<a name="apiUsuariosRemoverImagemPerfilDelete"></a>
# **apiUsuariosRemoverImagemPerfilDelete**
> List apiUsuariosRemoverImagemPerfilDelete()



### Parameters
This endpoint does not need any parameter.

### Return type

[**List**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiUsuariosUploadImagemPerfilPost"></a>
# **apiUsuariosUploadImagemPerfilPost**
> List apiUsuariosUploadImagemPerfilPost(Imagem)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **Imagem** | **File**|  | [default to null] |

### Return type

[**List**](../Models/UsuarioImagemResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: multipart/form-data
- **Accept**: text/plain, application/json, text/json

