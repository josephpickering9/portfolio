import type { TagTypeValue } from "~/types/TagTypeValue";
import { TagType } from "@api";

export const tagTypes: TagTypeValue[] = [
    { value: TagType.DEFAULT, label: 'Default', icon: 'heroicons:tag', color: 'text-gray-500' },
    { value: TagType.AI, label: 'AI', icon: 'heroicons:sparkles', color: 'text-purple-500' },
    { value: TagType.BACKEND, label: 'Backend', icon: 'heroicons:server', color: 'text-green-500' },
    { value: TagType.FRONTEND, label: 'Frontend', icon: 'heroicons:computer-desktop', color: 'text-blue-500' },
    { value: TagType.DEV_OPS, label: 'DevOps', icon: 'heroicons:circle-stack', color: 'text-orange-500' },
    { value: TagType.DATA, label: 'Data', icon: 'heroicons:chart-bar', color: 'text-cyan-500' },
    { value: TagType.CMS, label: 'CMS', icon: 'heroicons:document-text', color: 'text-pink-500' },
    { value: TagType.MOBILE, label: 'Mobile', icon: 'heroicons:device-phone-mobile', color: 'text-indigo-500' },
    { value: TagType.OTHER, label: 'Other', icon: 'heroicons:ellipsis-horizontal-circle', color: 'text-gray-400' },
]