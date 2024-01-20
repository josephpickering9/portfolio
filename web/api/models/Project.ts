/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Tag } from './Tag';
export type Project = {
    id: number;
    title: string;
    shortDescription: string;
    description: string;
    company: string;
    image?: string | null;
    backgroundImage?: string | null;
    year: number;
    website?: string | null;
    tags: Array<Tag>;
};

