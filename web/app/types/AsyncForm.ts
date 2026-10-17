export interface AsyncForm<T> {
    data?: T;
    error?: string;
    status?: number;
    loading?: boolean;
}
