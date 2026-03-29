import { type FormEvent, useEffect, useState } from "react";
import { X, Loader2 } from "lucide-react";
import type { CategoryDetails, CategoryRequest } from "./types";

interface CategoryFormProps {
  category: CategoryDetails | null;
  isLoading: boolean;
  isSaving: boolean;
  error: string | null;
  onSave: (request: CategoryRequest) => void;
  onClose: () => void;
}

const empty: CategoryRequest = {
  categoryName: "",
  description: null,
};

function toRequest(c: CategoryDetails): CategoryRequest {
  return {
    categoryName: c.categoryName,
    description: c.description,
  };
}

export function CategoryForm({
  category,
  isLoading,
  isSaving,
  error,
  onSave,
  onClose,
}: CategoryFormProps) {
  const [form, setForm] = useState<CategoryRequest>(empty);

  useEffect(() => {
    setForm(category ? toRequest(category) : empty);
  }, [category]);

  const set = (field: keyof CategoryRequest, value: string) => {
    setForm((prev) => ({
      ...prev,
      [field]: field === "categoryName" ? value : value || null,
    }));
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    onSave(form);
  };

  const isNew = category === null;

  return (
    <div className="fixed inset-0 z-50 flex justify-end">
      <div className="fixed inset-0 bg-black/30" onClick={onClose} />

      <div className="relative z-10 flex w-full max-w-md flex-col bg-white shadow-xl">
        <div className="flex items-center justify-between border-b px-6 py-4">
          <h2 className="text-lg font-semibold">
            {isNew ? "Add Category" : "Edit Category"}
          </h2>
          <button
            onClick={onClose}
            className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-700"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {isLoading ? (
          <div className="flex flex-1 items-center justify-center">
            <Loader2 className="h-6 w-6 animate-spin text-gray-400" />
          </div>
        ) : (
          <form
            onSubmit={handleSubmit}
            className="flex flex-1 flex-col overflow-y-auto"
          >
            <div className="flex-1 space-y-4 px-6 py-4">
              {error && (
                <p className="rounded bg-red-50 px-3 py-2 text-sm text-red-600">
                  {error}
                </p>
              )}

              {!isNew && category && (
                <div className="flex items-center gap-2">
                  <span className="text-sm font-medium text-gray-700">ID</span>
                  <span className="inline-flex h-7 items-center rounded-md bg-gray-100 px-2.5 text-sm font-mono text-gray-500">
                    {category.categoryId}
                  </span>
                </div>
              )}

              <label className="block">
                <span className="mb-1 block text-sm font-medium text-gray-700">
                  Category Name<span className="text-red-500"> *</span>
                </span>
                <input
                  type="text"
                  value={form.categoryName}
                  onChange={(e) => set("categoryName", e.target.value)}
                  required
                  className="w-full rounded-md border px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none"
                />
              </label>

              <label className="block">
                <span className="mb-1 block text-sm font-medium text-gray-700">
                  Description
                </span>
                <textarea
                  value={form.description ?? ""}
                  onChange={(e) => set("description", e.target.value)}
                  rows={3}
                  className="w-full rounded-md border px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none"
                />
              </label>
            </div>

            <div className="flex items-center justify-end gap-3 border-t px-6 py-4">
              <button
                type="button"
                onClick={onClose}
                className="rounded-md border px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isSaving || !form.categoryName.trim()}
                className="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
              >
                {isSaving && <Loader2 className="h-4 w-4 animate-spin" />}
                Save
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}
