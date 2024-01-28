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
    public static getProjectId(
        id?: number,
    ): CancelablePromise<Project> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/project/id',
            query: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @returns any Success
     * @throws ApiError
     */
    public static deleteProjectId(
        id?: number,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/project/id',
            query: {
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
}
