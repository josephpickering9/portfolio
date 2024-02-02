/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Company } from '../models/Company';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CompanyService {
    /**
     * @param search
     * @returns Company Success
     * @throws ApiError
     */
    public static getCompany(
        search?: string,
    ): CancelablePromise<Array<Company>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/company',
            query: {
                'search': search,
            },
        });
    }
    /**
     * @param formData
     * @returns Company Success
     * @throws ApiError
     */
    public static postCompany(
        formData?: any,
    ): CancelablePromise<Company> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/company',
            formData: formData,
            mediaType: 'multipart/form-data',
        });
    }
    /**
     * @param id
     * @returns Company Success
     * @throws ApiError
     */
    public static getCompany1(
        id: number,
    ): CancelablePromise<Company> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/company/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param formData
     * @returns Company Success
     * @throws ApiError
     */
    public static putCompany(
        id: number,
        formData?: any,
    ): CancelablePromise<Company> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/company/{id}',
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
    public static deleteCompany(
        id: number,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/company/{id}',
            path: {
                'id': id,
            },
        });
    }
}
