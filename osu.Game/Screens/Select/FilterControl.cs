﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Select.Filter;
using Container = osu.Framework.Graphics.Containers.Container;
using osu.Framework.Input;
using osu.Game.Database;
using osu.Framework.Graphics.Shapes;

namespace osu.Game.Screens.Select
{
    public class FilterControl : Container
    {
        public Action<FilterCriteria> FilterChanged;

        private readonly OsuTabControl<SortMode> sortTabs;

        private readonly TabControl<GroupMode> groupTabs;

        private SortMode sort = SortMode.Title;
        public SortMode Sort
        {
            get { return sort; }
            set
            {
                if (sort != value)
                {
                    sort = value;
                    FilterChanged?.Invoke(CreateCriteria());
                }
            }
        }

        private GroupMode group = GroupMode.All;
        public GroupMode Group
        {
            get { return group; }
            set
            {
                if (group != value)
                {
                    group = value;
                    FilterChanged?.Invoke(CreateCriteria());
                }
            }
        }

        public FilterCriteria CreateCriteria() => new FilterCriteria
        {
            Group = group,
            Sort = sort,
            SearchText = searchTextBox.Text,
            Ruleset = ruleset
        };

        public Action Exit;

        private readonly SearchTextBox searchTextBox;

        public override bool Contains(Vector2 screenSpacePos) => base.Contains(screenSpacePos) || groupTabs.Contains(screenSpacePos) || sortTabs.Contains(screenSpacePos);

        public FilterControl()
        {
            Children = new Drawable[]
            {
                new Box
                {
                    Colour = Color4.Black,
                    Alpha = 0.8f,
                    RelativeSizeAxes = Axes.Both,
                },
                new Container
                {
                    Padding = new MarginPadding(20),
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.5f,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Children = new Drawable[]
                    {
                        searchTextBox = new SearchTextBox
                        {
                            RelativeSizeAxes = Axes.X,
                            Exit = () => Exit?.Invoke(),
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 1,
                            Colour = OsuColour.Gray(80),
                            Origin = Anchor.BottomLeft,
                            Anchor = Anchor.BottomLeft,
                        },
                        new FillFlowContainer
                        {
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            Direction = FillDirection.Horizontal,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new Drawable[]
                            {
                                groupTabs = new OsuTabControl<GroupMode>
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 24,
                                    Width = 0.5f,
                                    AutoSort = true
                                },
                                //spriteText = new OsuSpriteText
                                //{
                                //    Font = @"Exo2.0-Bold",
                                //    Text = "Sort results by",
                                //    TextSize = 14,
                                //    Margin = new MarginPadding
                                //    {
                                //        Top = 5,
                                //        Bottom = 5
                                //    },
                                //},
                                sortTabs = new OsuTabControl<SortMode>()
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Width = 0.5f,
                                    Height = 24,
                                    AutoSort = true,
                                }
                            }
                        },
                    }
                }
            };

            searchTextBox.Current.ValueChanged += t => FilterChanged?.Invoke(CreateCriteria());

            groupTabs.PinItem(GroupMode.All);
            groupTabs.PinItem(GroupMode.RecentlyPlayed);
            groupTabs.Current.ValueChanged += val => Group = val;
            sortTabs.Current.ValueChanged += val => Sort = val;
        }

        public void Deactivate()
        {
            searchTextBox.HoldFocus = false;
            if (searchTextBox.HasFocus)
                inputManager.ChangeFocus(searchTextBox);
        }

        public void Activate()
        {
            searchTextBox.HoldFocus = true;
        }

        private readonly Bindable<RulesetInfo> ruleset = new Bindable<RulesetInfo>();

        private InputManager inputManager;

        [BackgroundDependencyLoader(permitNulls: true)]
        private void load(OsuColour colours, OsuGame osu, UserInputManager inputManager)
        {
            this.inputManager = inputManager;

            sortTabs.AccentColour = colours.GreenLight;

            if (osu != null)
                ruleset.BindTo(osu.Ruleset);
            ruleset.ValueChanged += val => FilterChanged?.Invoke(CreateCriteria());
            ruleset.TriggerChange();
        }

        protected override bool OnMouseDown(InputState state, MouseDownEventArgs args) => true;

        protected override bool OnMouseMove(InputState state) => true;

        protected override bool OnClick(InputState state) => true;

        protected override bool OnDragStart(InputState state) => true;
    }
}