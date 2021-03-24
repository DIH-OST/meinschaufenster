// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.Effects
{
    /// <summary>
    /// <para>ThemeEffects</para>
    /// Klasse ThemeEffects. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>

    //public static class ThemeEffects
    //{
    //    public static readonly BindableProperty CircleProperty =
    //        BindableProperty.CreateAttached("Circle", typeof(bool), typeof(ThemeEffects), false, propertyChanged: OnChanged<CircleEffect, bool>);

    //    public static bool GetCircle(BindableObject view)
    //    {
    //        return (bool) view.GetValue(CircleProperty);
    //    }

    //    public static void SetCircle(BindableObject view, bool circle)
    //    {
    //        view.SetValue(CircleProperty, circle);
    //    }


    //    private static void OnChanged<TEffect, TProp>(BindableObject bindable, object oldValue, object newValue)
    //        where TEffect : Effect, new()
    //    {
    //        var view = bindable as Xamarin.Forms.View;
    //        if (view == null)
    //        {
    //            return;
    //        }

    //        if (EqualityComparer<TProp>.Equals(newValue, default(TProp)))
    //        {
    //            var toRemove = view.Effects.FirstOrDefault(e => e is TEffect);
    //            if (toRemove != null)
    //            {
    //                view.Effects.Remove(toRemove);
    //            }
    //        }
    //        else
    //        {
    //            view.Effects.Add(new TEffect());
    //        }
    //    }

    //    private class CircleEffect : RoutingEffect
    //    {
    //        public CircleEffect()
    //            : base("eShopOnContainers.CircleEffect")
    //        {
    //        }
    //    }
    //}
}