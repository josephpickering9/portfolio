/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Company } from './Company';
import type { ProjectImage } from './ProjectImage';
import type { ProjectRepository } from './ProjectRepository';
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
    showMockup: boolean;
    slug: string;
    images: Array<ProjectImage>;
    repositories: Array<ProjectRepository>;
    tags: Array<Tag>;
    relatedProjects?: Array<Project>;
    logo?: ProjectImage;
    readonly logoUrl?: string | null;
    card?: ProjectImage;
    readonly cardUrl?: string | null;
    banner?: ProjectImage;
    readonly bannerUrl?: string | null;
};

