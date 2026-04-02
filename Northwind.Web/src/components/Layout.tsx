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
  LogIn,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { useHealthCheck } from "@/hooks/useHealthCheck";

const homeItems = [
  { to: "/", label: "Home", icon: LayoutDashboard, health: "/api/health" },
];

const navItems = [
  { to: "/catalog", label: "Catalog", icon: Package, health: "/api/catalog/health", countKey: "products" },
  { to: "/crm", label: "CRM", icon: Users, health: "/api/crm/health" },
  { to: "/fulfillment", label: "Fulfillment", icon: Truck, health: "/api/fulfillment/health" },
  { to: "/sales-ordering", label: "Sales Ordering", icon: ShoppingCart, health: "/api/sales-ordering/health" },
  { to: "/sales-org", label: "Sales Org", icon: Building2, health: "/api/sales-org/health" },
  { to: "/purchasing", label: "Purchasing", icon: Factory, health: "/api/purchasing/health" },
];

const reportingItems = [
  { to: "/reporting", label: "Reporting", icon: BarChart3, health: "/api/reporting/health" },
];

function NavItem({ to, label, icon: Icon, health, countKey = "count" }: (typeof navItems)[number]) {
  const { data, isError } = useHealthCheck(health);
  const count = data != null ? (data as Record<string, unknown>)[countKey] : undefined;

  return (
    <NavLink
      to={to}
      end={to === "/"}
      className={({ isActive }) =>
        cn(
          "flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium transition-colors",
          isActive
            ? "bg-slate-700 text-white"
            : "text-slate-400 hover:bg-slate-800 hover:text-white"
        )
      }
    >
      <Icon className="h-4 w-4" />
      {label}
      {isError && <AlertCircle className="ml-auto h-4 w-4 shrink-0 text-red-500" />}
      {!isError && count != null && (
        <span className="ml-auto inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-indigo-500 px-1.5 text-xs font-semibold text-white">
          {count as number}
        </span>
      )}
    </NavLink>
  );
}

export function Layout() {
  return (
    <div className="flex h-screen flex-col">
      <header className="flex h-13 shrink-0 items-center justify-between border-b bg-white px-6 shadow-sm">
        <span className="text-base font-bold tracking-tight text-slate-800">Northwind</span>
        <button className="inline-flex items-center gap-2 rounded-md border border-slate-200 bg-white px-3 py-1.5 text-sm font-medium text-slate-600 shadow-sm hover:bg-slate-50 hover:text-slate-900 transition-colors">
          <LogIn className="h-4 w-4" />
          Login
        </button>
      </header>

      <div className="flex flex-1 overflow-hidden">
        <aside className="flex w-56 flex-col bg-slate-900">
          <nav className="flex-1 space-y-0.5 p-3">
            {homeItems.map((item) => (
              <NavItem key={item.to} {...item} />
            ))}
            <div className="my-3 border-t border-slate-700" />
            {navItems.map((item) => (
              <NavItem key={item.to} {...item} />
            ))}
            <div className="my-3 border-t border-slate-700" />
            {reportingItems.map((item) => (
              <NavItem key={item.to} {...item} />
            ))}
          </nav>
        </aside>
        <main className="flex-1 overflow-auto bg-slate-50 p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
