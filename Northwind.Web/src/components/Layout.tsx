import { NavLink, Outlet } from "react-router-dom";
import {
  LayoutDashboard,
  Package,
  Users,
  Truck,
  BarChart3,
  ShoppingCart,
  Building2,
  Factory,
  AlertCircle,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { useHealthCheck } from "@/hooks/useHealthCheck";

const navItems = [
  { to: "/", label: "Home", icon: LayoutDashboard, health: "/api/health" },
  { to: "/catalog", label: "Catalog", icon: Package, health: "/api/catalog/health" },
  { to: "/crm", label: "CRM", icon: Users, health: "/api/crm/health" },
  { to: "/fulfillment", label: "Fulfillment", icon: Truck, health: "/api/fulfillment/health" },
  { to: "/reporting", label: "Reporting", icon: BarChart3, health: "/api/reporting/health" },
  { to: "/sales-ordering", label: "Sales Ordering", icon: ShoppingCart, health: "/api/sales-ordering/health" },
  { to: "/sales-org", label: "Sales Org", icon: Building2, health: "/api/sales-org/health" },
  { to: "/supplier", label: "Supplier", icon: Factory, health: "/api/supplier/health" },
];

function NavItem({ to, label, icon: Icon, health }: (typeof navItems)[number]) {
  const { data, isError } = useHealthCheck(health);

  return (
    <NavLink
      to={to}
      end={to === "/"}
      className={({ isActive }) =>
        cn(
          "flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium transition-colors",
          isActive
            ? "bg-gray-200 text-gray-900"
            : "text-gray-600 hover:bg-gray-100 hover:text-gray-900"
        )
      }
    >
      <Icon className="h-4 w-4" />
      {label}
      {isError && <AlertCircle className="ml-auto h-4 w-4 shrink-0 text-red-500" />}
      {!isError && data?.count != null && (
        <span className="ml-auto inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-blue-600 px-1.5 text-xs font-semibold text-white">
          {data.count}
        </span>
      )}
    </NavLink>
  );
}

export function Layout() {
  return (
    <div className="flex h-screen">
      <aside className="flex w-56 flex-col border-r bg-gray-50">
        <div className="border-b px-4 py-4">
          <h1 className="text-lg font-bold tracking-tight">Northwind</h1>
        </div>
        <nav className="flex-1 space-y-1 p-2">
          {navItems.map((item) => (
            <NavItem key={item.to} {...item} />
          ))}
        </nav>
      </aside>
      <main className="flex-1 overflow-auto p-6">
        <Outlet />
      </main>
    </div>
  );
}
