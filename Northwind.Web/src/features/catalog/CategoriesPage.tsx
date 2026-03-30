import { useState } from "react";
import { Plus, PackageOpen } from "lucide-react";
import { useCategories, useCategory, useCreateCategory, useUpdateCategory } from "./useCategoryApi";
import { CategoryTable, CategoryTableSkeleton } from "./CategoryTable";
import { CategoryForm } from "./CategoryForm";
import type { CategoryRequest } from "./types";

type PanelState =
  | { mode: "closed" }
  | { mode: "add" }
  | { mode: "edit"; categoryId: number };

export function CategoriesPage() {
  const [panel, setPanel] = useState<PanelState>({ mode: "closed" });

  const { data, isLoading, isError, error } = useCategories();

  const editingId = panel.mode === "edit" ? panel.categoryId : null;
  const { data: editCategory, isLoading: isLoadingDetail } = useCategory(editingId);

  const createMutation = useCreateCategory();
  const updateMutation = useUpdateCategory();

  const activeMutation = panel.mode === "add" ? createMutation : updateMutation;

  const handleSave = (request: CategoryRequest) => {
    if (panel.mode === "add") {
      createMutation.mutate(request, {
        onSuccess: () => setPanel({ mode: "closed" }),
      });
    } else if (panel.mode === "edit") {
      updateMutation.mutate(
        { categoryId: panel.categoryId, request },
        { onSuccess: () => setPanel({ mode: "closed" }) },
      );
    }
  };

  const categories = data ?? [];

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h2 className="text-lg font-semibold">Categories</h2>
        <button
          onClick={() => setPanel({ mode: "add" })}
          className="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
        >
          <Plus className="h-4 w-4" />
          Add Category
        </button>
      </div>

      {isLoading && <CategoryTableSkeleton />}

      {isError && (
        <p className="text-sm text-red-600">
          Failed to load categories:{" "}
          {error instanceof Error ? error.message : "Unknown error"}
        </p>
      )}

      {data && categories.length === 0 && (
        <div className="flex flex-col items-center justify-center rounded-lg border border-dashed py-16 text-center">
          <PackageOpen className="mb-4 h-12 w-12 text-gray-300" />
          <p className="mb-1 text-lg font-medium text-gray-600">
            No categories yet
          </p>
          <p className="mb-4 text-sm text-gray-400">
            Get started by adding your first category.
          </p>
          <button
            onClick={() => setPanel({ mode: "add" })}
            className="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            <Plus className="h-4 w-4" />
            Add Category
          </button>
        </div>
      )}

      {data && categories.length > 0 && (
        <CategoryTable
          categories={categories}
          onEdit={(id) => setPanel({ mode: "edit", categoryId: id })}
        />
      )}

      {panel.mode !== "closed" && (
        <CategoryForm
          category={panel.mode === "edit" ? editCategory ?? null : null}
          isLoading={panel.mode === "edit" && isLoadingDetail}
          isSaving={activeMutation.isPending}
          error={
            activeMutation.isError
              ? activeMutation.error instanceof Error
                ? activeMutation.error.message
                : "Something went wrong"
              : null
          }
          onSave={handleSave}
          onClose={() => setPanel({ mode: "closed" })}
        />
      )}
    </div>
  );
}
