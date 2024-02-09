/* generated using openapi-typescript-codegen -- do no edit */
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
     * @returns Project Success
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
     * @returns Project Success
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
     * @returns Project Success
     * @throws ApiError
     */
    public static getProject1(
        id: number,
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
     * @returns Project Success
     * @throws ApiError
     */
    public static putProject(
        id: number,
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
     * @returns any Success
     * @throws ApiError
     */
    public static deleteProject(
        id: number,
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
     * @returns Project Success
     * @throws ApiError
     */
    public static getProjectSlug(
        slug: string,
    ): CancelablePromise<Project> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/project/slug/{slug}',
            path: {
                'slug': slug,
            },
        });
    }
    /**
     * @param id
     * @returns Project Success
     * @throws ApiError
     */
    public static getProjectRelated(
        id: number,
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
