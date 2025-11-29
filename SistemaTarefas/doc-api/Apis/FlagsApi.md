# FlagsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiFlagsGet**](FlagsApi.md#apiFlagsGet) | **GET** /api/Flags |  |
| [**apiFlagsIdDelete**](FlagsApi.md#apiFlagsIdDelete) | **DELETE** /api/Flags/{id} |  |
| [**apiFlagsIdGet**](FlagsApi.md#apiFlagsIdGet) | **GET** /api/Flags/{id} |  |
| [**apiFlagsIdPut**](FlagsApi.md#apiFlagsIdPut) | **PUT** /api/Flags/{id} |  |
| [**apiFlagsPost**](FlagsApi.md#apiFlagsPost) | **POST** /api/Flags |  |


<a name="apiFlagsGet"></a>
# **apiFlagsGet**
> List apiFlagsGet(rotuloFlag)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **rotuloFlag** | **String**|  | [optional] [default to null] |

### Return type

[**List**](../Models/FlagResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiFlagsIdDelete"></a>
# **apiFlagsIdDelete**
> ResponseModel apiFlagsIdDelete(id)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **id** | **Integer**|  | [default to null] |

### Return type

[**ResponseModel**](../Models/ResponseModel.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiFlagsIdGet"></a>
# **apiFlagsIdGet**
> FlagResponse apiFlagsIdGet(id, rotuloFlag)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **id** | **Integer**|  | [default to null] |
| **rotuloFlag** | **String**|  | [optional] [default to null] |

### Return type

[**FlagResponse**](../Models/FlagResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

<a name="apiFlagsIdPut"></a>
# **apiFlagsIdPut**
> FlagResponse apiFlagsIdPut(id, FlagRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **id** | **Integer**|  | [default to null] |
| **FlagRequest** | [**FlagRequest**](../Models/FlagRequest.md)|  | [optional] |

### Return type

[**FlagResponse**](../Models/FlagResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

<a name="apiFlagsPost"></a>
# **apiFlagsPost**
> FlagResponse apiFlagsPost(FlagRequest)



### Parameters

|Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **FlagRequest** | [**FlagRequest**](../Models/FlagRequest.md)|  | [optional] |

### Return type

[**FlagResponse**](../Models/FlagResponse.md)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

