/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Project } from '../models/Project';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class ProjectService {
    /**
     * @param search
     * @returns Project OK
     * @throws ApiError
     */
    public static getProject(
        search?: string,
    ): CancelablePromise<Array<Project>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/project',
            query: {
                'search': search,
            },
        });
    }
    /**
     * @param formData
     * @returns Project OK
     * @throws ApiError
     */
    public static postProject(
        formData?: any,
    ): CancelablePromise<Project> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/project',
            formData: formData,
            mediaType: 'multipart/form-data',
        });
    }
    /**
     * @param id
     * @returns Project OK
     * @throws ApiError
     */
    public static getProject1(
        id: string,
    ): CancelablePromise<Project> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/project/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param formData
     * @returns Project OK
     * @throws ApiError
     */
    public static putProject(
        id: string,
        formData?: any,
    ): CancelablePromise<Project> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/project/{id}',
            path: {
                'id': id,
            },
            formData: formData,
            mediaType: 'multipart/form-data',
        });
    }
    /**
     * @param id
     * @returns any OK
     * @throws ApiError
     */
    public static deleteProject(
        id: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/project/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param slug
     * @returns Project OK
     * @throws ApiError
     */
    public static getProject2(
        slug: string,
    ): CancelablePromise<Project> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/project/{slug}',
            path: {
                'slug': slug,
            },
        });
    }
    /**
     * @param id
     * @returns Project OK
     * @throws ApiError
     */
    public static getProjectRelated(
        id: string,
    ): CancelablePromise<Array<Project>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/project/{id}/related',
            path: {
                'id': id,
            },
        });
    }
}
