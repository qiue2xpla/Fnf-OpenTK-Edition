using Fnf.Framework.TrueType.Rasterization;
using Fnf.Framework.Graphics;
using Fnf.Framework;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.ComponentModel;
using System.Linq;
using System;
using OpenTK.Graphics.ES11;

namespace Fnf.Framework
{
    public class TabView : GUI, IRenderable, IUpdatable
    {
        public bool isUpdatable { get; set; } = true;
        public bool isRenderable { get; set; } = true;

        /// <summary>
        /// The color of the whole GUI background
        /// </summary>
        public Color backgroundColor = Color.White;

        /// <summary>
        /// The color of the items text
        /// </summary>
        public Color textColor { get => tabTitle.color; set => tabTitle.color = value; }

        /// <summary>
        /// The default item color when unselected
        /// </summary>
        public Color defaultItemColor = new Color(0.96f);

        public Color hoverItemColor = new Color(0.92f);

        public Color pressedItemColor = new Color(0.88f);

        public Color selectedItemColorMultiplier = new Color(0.9f);

        public float cornerRadius = 4;
        public float borderSmoothness = 2;
        public float padding = 3;
        public float fontSize { get => tabTitle.fontSize; set { tabTitle.fontSize = value; RefreshItems(null, null); } }

        public ObservableCollection<TabViewItem> items;
        public int selectedItem;

        GUI contentArea;
        Text tabTitle;

        public TabView(FontAtlas atlas)
        {
            width = 100;
            height = 100;

            items = new ObservableCollection<TabViewItem>();

            contentArea = new GUI();
            contentArea.parent = this;

            tabTitle = new Text(atlas);
            tabTitle.parent = this;
            tabTitle.fitContent = true;
            tabTitle.color = Color.Black;
            tabTitle.textAlignment = TextAlignment.Center;

            items.CollectionChanged += RefreshItems;
        }

        public void Update()
        {
            contentArea.width = width - cornerRadius * 2;
            contentArea.height = height - fontSize*2 - cornerRadius;
            contentArea.localPosition.y = -fontSize + cornerRadius / 2;

            for (int i = 0; i < items.Count; i++)
            {
                TabViewItem item = items[i];
                tabTitle.width = item.size.x - padding * 2;
                tabTitle.height = item.size.y - padding * 2;
                tabTitle.localPosition = item.pos;

                if (tabTitle.IsOverGUI())
                {
                    if(Input.GetButton(MouseButton.Left))
                    {
                        item.color = pressedItemColor;

                        if (Input.GetButtonDown(MouseButton.Left))
                        {
                            selectedItem = i;
                        }
                    }
                    else
                    {
                        item.color = hoverItemColor;
                    }
                }
                else
                {
                    item.color = defaultItemColor;
                }

                if (i == selectedItem) item.color *= selectedItemColorMultiplier;
            }

            if (selectedItem < items.Count)
            {
                TabViewItem item = items[selectedItem];
                if (item.content != null)
                {
                    item.content.parent = contentArea;
                    item.content.width = contentArea.width;
                    item.content.height = contentArea.height;
                    item.content.localPosition = Vector2.Zero;
                    item.content.localRotation = 0;
                    item.content.localScale = Vector2.One;

                    if (item.content is IUpdatable updatable) updatable.Update();
                }
            }
        }

        public void Render()
        {
            if (!isRenderable) return;
            if (IsOverGUI()) RaycastHit();

            // Background
            Gizmos.DrawRoundQuad(this, backgroundColor, cornerRadius, borderSmoothness);

            // Tabs
            for (int i = 0; i < items.Count; i++)
            {
                tabTitle.text = items[i].title;
                tabTitle.width = items[i].size.x - padding * 2;
                tabTitle.height = items[i].size.y - padding * 2;
                tabTitle.localPosition = items[i].pos;
                Gizmos.DrawRoundQuad(tabTitle, items[i].color, cornerRadius, borderSmoothness);
                tabTitle.Render();
            }

            // Content
            if (items[selectedItem].content is IRenderable renderable) renderable.Render();
        }

        void RefreshItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            Vector2 drawingPoint = new Vector2(-width / 2, height / 2);
            for (int i = 0; i < items.Count; i++)
            {
                tabTitle.text = items[i].title;

                Vector2 size = new Vector2(tabTitle.width + 12, fontSize * 2);

                // TODO: Add masking or sum
                if (drawingPoint.x + size.x > width / 2)
                {
                    throw new NotImplementedException("Multiline TabView is not supported");
                }

                items[i].size = size;
                items[i].pos = drawingPoint + size * new Vector2(0.5f, -0.5f);

                drawingPoint.x += size.x;
            }
        }
    }

    public class TabViewItem : INotifyPropertyChanged
    {
        public string title 
        { 
            get => _title; 
            set 
            {
                if (value == _title) return;
                title = value;
                OnPropertyChanged(nameof(title));
            } 
        }


        public GUI content { get => _content; set { _content = value; OnPropertyChanged(nameof(content)); } }

        string _title;
        GUI _content;

        // Used for rendering
        internal Color color;
        internal Vector2 pos;
        internal Vector2 size;

        public TabViewItem(string title, GUI content)
        {
            _title = title;
            _content = content;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}