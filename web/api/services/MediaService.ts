/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class MediaService {
    /**
     * @param fileName
     * @returns any Success
     * @throws ApiError
     */
    public static getMediaUploads(
        fileName: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/media/uploads/{fileName}',
            path: {
                'fileName': fileName,
            },
        });
    }
}
