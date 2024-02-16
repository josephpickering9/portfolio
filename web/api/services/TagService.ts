/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CreateTag } from '../models/CreateTag';
import type { Tag } from '../models/Tag';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class TagService {
    /**
     * @param search
     * @returns Tag Success
     * @throws ApiError
     */
    public static getTag(
        search?: string,
    ): CancelablePromise<Array<Tag>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/tag',
            query: {
                'search': search,
            },
        });
    }
    /**
     * @param requestBody
     * @returns Tag Success
     * @throws ApiError
     */
    public static postTag(
        requestBody?: CreateTag,
    ): CancelablePromise<Tag> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/tag',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns Tag Success
     * @throws ApiError
     */
    public static getTag1(
        id: number,
    ): CancelablePromise<Tag> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/tag/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns Tag Success
     * @throws ApiError
     */
    public static putTag(
        id: number,
        requestBody?: CreateTag,
    ): CancelablePromise<Tag> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/tag/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns any Success
     * @throws ApiError
     */
    public static deleteTag(
        id: number,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/tag/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param slug
     * @returns Tag Success
     * @throws ApiError
     */
    public static getTag2(
        slug: string,
    ): CancelablePromise<Tag> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/tag/{slug}',
            path: {
                'slug': slug,
            },
        });
    }
}
