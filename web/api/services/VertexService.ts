/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { VertexQueryRequest } from '../models/VertexQueryRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class VertexService {
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static postVertexIngest(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/vertex/ingest',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static postVertexQuery(
        requestBody?: VertexQueryRequest,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/vertex/query',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
}
