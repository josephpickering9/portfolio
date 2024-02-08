/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Company } from './Company';
import type { ProjectImage } from './ProjectImage';
import type { Tag } from './Tag';
export type Project = {
    id: number;
    title: string;
    shortDescription: string;
    description: string;
    companyId?: number | null;
    company?: Company;
    year: number;
    website?: string | null;
    slug: string;
    images: Array<ProjectImage>;
    tags: Array<Tag>;
    logo?: ProjectImage;
    readonly logoUrl?: string | null;
    card?: ProjectImage;
    readonly cardUrl?: string | null;
    banner?: ProjectImage;
    readonly bannerUrl?: string | null;
};

