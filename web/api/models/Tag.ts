/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Project } from './Project';
import type { TagType } from './TagType';
export type Tag = {
    id: number;
    title: string;
    type: TagType;
    icon?: string | null;
    customColour?: string | null;
    projects: Array<Project>;
};

