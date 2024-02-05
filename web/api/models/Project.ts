/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Company } from './Company';
import type { Tag } from './Tag';
export type Project = {
    id: number;
    title: string;
    shortDescription: string;
    description: string;
    companyId?: number | null;
    company?: Company;
    image?: string | null;
    backgroundImage?: string | null;
    year: number;
    website?: string | null;
    slug: string;
    tags: Array<Tag>;
};

