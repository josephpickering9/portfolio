/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CreateProjectImage } from './CreateProjectImage';
import type { CreateProjectRepository } from './CreateProjectRepository';
export type CreateProject = {
    title: string;
    shortDescription: string;
    description: string;
    companyId?: string | null;
    startDate: string;
    endDate?: string | null;
    website?: string | null;
    showMockup: boolean;
    images?: Array<CreateProjectImage>;
    tags: Array<string>;
    repositories?: Array<CreateProjectRepository>;
};

