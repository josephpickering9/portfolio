import type { TagTypeValue } from "~/types/TagTypeValue";
import { TagType } from "@api";

export const tagTypes: TagTypeValue[] = [
    { value: TagType.DEFAULT, label: 'Default', icon: 'heroicons:tag' },
    { value: TagType.AI, label: 'AI', icon: 'heroicons:sparkles' },
    { value: TagType.BACKEND, label: 'Backend', icon: 'heroicons:server' },
    { value: TagType.FRONTEND, label: 'Frontend', icon: 'heroicons:computer-desktop' },
    { value: TagType.DEV_OPS, label: 'DevOps', icon: 'heroicons:circle-stack' },
    { value: TagType.DATA, label: 'Data', icon: 'heroicons:chart-bar' },
    { value: TagType.CMS, label: 'CMS', icon: 'heroicons:document-text' },
    { value: TagType.MOBILE, label: 'Mobile', icon: 'heroicons:device-phone-mobile' },
    { value: TagType.OTHER, label: 'Other', icon: 'heroicons:ellipsis-horizontal-circle' },
]