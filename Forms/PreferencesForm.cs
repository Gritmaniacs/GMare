﻿#region MIT

// 
// GMare.
// Copyright (C) 2011, 2012, 2013, 2014 Michael Mercado
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

#endregion

using System;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Windows.Forms;
using System.Collections.Generic;
using GMare.Objects;

namespace GMare.Forms
{
    /// <summary>
    /// Dialog to change app settings for vaious UI properties
    /// </summary>
    public partial class PreferencesForm : Form
    {
        #region Fields

        private int _undoRedoMaximum = 0;         // The maximum amount allowed to undo/redo
        private float _brightness = 0;            // The brightness of lower layers
        private float _transparency = 0;          // The transparency of higher layers
        private bool _updateTextures = false;     // If texture updates are required
        private bool _updateUndoRedo = false;     // If undo/redo maximum update is required
        private bool _showTips = true;            // If showing GMare tips
        private bool _showLayerCursorTip = true;  // If showing layer cursor tip
        private float _scaleX = 1;                // The horizontal scale multiplier
        private float _scaleY = 1;                // The vertical scale multiplier

        #endregion

        #region Properties

        /// <summary>
        /// Gets if undo/redo maximum update is required
        /// </summary>
        public bool UpdateUnodRedo
        {
            get { return _updateUndoRedo; }
        }

        /// <summary>
        /// Gets if texture updates are required
        /// </summary>
        public bool UpdateTextures
        {
            get { return _updateTextures; }
        }

        /// <summary>
        /// Gets if using the area grid
        /// </summary>
        public bool UseAreaGrid
        {
            get { return grpAreaGrid.Checked; }
        }

        /// <summary>
        /// Gets the area size
        /// </summary>
        public Size AreaSize
        {
            get { return new Size((int)nudGridWidth.Value, (int)nudGridHeight.Value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new preferences dialog
        /// </summary>
        public PreferencesForm(bool useAreaGrid, int areaWidth, int areaHeight)
        {
            InitializeComponent();

            // Get app settings
            _undoRedoMaximum = App.GetConfigInt(App.UndoRedoMaximumAppKey, App.UndoRedoMaximumAppDefault);
            _brightness = App.GetConfigFloat(App.LowerLayerBrightnessAppKey, App.LowerLayerBrightnessAppDefault);
            _transparency = App.GetConfigFloat(App.UpperLayerTransparencyAppKey, App.UpperLayerTransparencyAppDefault);
            _showTips = App.GetConfigBool(App.ShowTipsAppKey, App.ShowTipsAppDefault);
            _showLayerCursorTip = App.GetConfigBool(App.ShowLayerCursorTipAppKey, App.ShowLayerCursorTipAppDefault);
            _scaleX = App.GetConfigFloat(App.ScaleXAppKey, App.ScaleXAppDefault);
            _scaleY = App.GetConfigFloat(App.ScaleYAppKey, App.ScaleYAppDefault);

            // Set UI
            nudMaximumUndoRedo.Value = _undoRedoMaximum;
            nudLowerLayerBrightness.Value = (decimal)_brightness;
            nudUpperLayerTransparency.Value = (decimal)_transparency;
            chkShowTips.Checked = _showTips;
            chkShowLayerCursorTip.Checked = _showLayerCursorTip;
            grpAreaGrid.Checked = useAreaGrid;
            nudGridWidth.Value = areaWidth;
            nudGridHeight.Value = areaHeight;
            nudHorizontalMagnificationMultiplier.Value = (decimal)_scaleX;
            nudVerticalMagnificationMultiplier.Value = (decimal)_scaleY;

            // Trigger area grid change
            nudAreaGrid_ValueChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Events

        /// <summary>
        /// Area size value changed
        /// </summary>
        private void nudAreaGrid_ValueChanged(object sender, EventArgs e)
        {
            int cols = App.Room.Backgrounds.Count > 0 ? (int)nudGridWidth.Value / App.Room.Backgrounds[0].TileSize.Width : 0;
            int rows = App.Room.Backgrounds.Count > 0 ? (int)nudGridHeight.Value / App.Room.Backgrounds[0].TileSize.Height : 0;
            lblGridSize.Text = "Grid Size: Columns: " + cols + " Rows: " + rows;
        }

        /// <summary>
        /// Button Ok click
        /// </summary>
        private void butOk_Click(object sender, EventArgs e)
        {
            // Get the config file
            Configuration config = App.GetConfig(false);

            // If the config file was not found, return
            if (config == null || config.AppSettings.Settings.Count == 0)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }

            try
            {
                // The list of existing keys
                List<string> keys = new List<string>(config.AppSettings.Settings.AllKeys);

                // Undeo/Redo maximum
                if (keys.Contains(App.UndoRedoMaximumAppKey))
                    config.AppSettings.Settings[App.UndoRedoMaximumAppKey].Value = nudMaximumUndoRedo.Value.ToString();
                else
                    config.AppSettings.Settings.Add(App.UndoRedoMaximumAppKey, nudMaximumUndoRedo.Value.ToString());

                // Lower layer brightness
                if (keys.Contains(App.LowerLayerBrightnessAppKey))
                    config.AppSettings.Settings[App.LowerLayerBrightnessAppKey].Value = nudLowerLayerBrightness.Value.ToString();
                else
                    config.AppSettings.Settings.Add(App.LowerLayerBrightnessAppKey, nudLowerLayerBrightness.Value.ToString());

                // Upper layer transparency
                if (keys.Contains(App.UpperLayerTransparencyAppKey))
                    config.AppSettings.Settings[App.UpperLayerTransparencyAppKey].Value = nudUpperLayerTransparency.Value.ToString();
                else
                    config.AppSettings.Settings.Add(App.UpperLayerTransparencyAppKey, nudUpperLayerTransparency.Value.ToString());

                // Show GMare tips
                if (keys.Contains(App.ShowTipsAppKey))
                    config.AppSettings.Settings[App.ShowTipsAppKey].Value = chkShowTips.Checked.ToString();
                else
                    config.AppSettings.Settings.Add(App.ShowLayerCursorTipAppKey, chkShowTips.Checked.ToString());

                // Show layer cursor tip
                if (keys.Contains(App.ShowLayerCursorTipAppKey))
                    config.AppSettings.Settings[App.ShowLayerCursorTipAppKey].Value = chkShowLayerCursorTip.Checked.ToString();
                else
                    config.AppSettings.Settings.Add(App.ShowLayerCursorTipAppKey, chkShowLayerCursorTip.Checked.ToString());

                // Horizontal scale multiplier
                if (keys.Contains(App.ScaleXAppKey))
                    config.AppSettings.Settings[App.ScaleXAppKey].Value = nudHorizontalMagnificationMultiplier.Value.ToString();
                else
                    config.AppSettings.Settings.Add(App.ScaleXAppKey, nudHorizontalMagnificationMultiplier.Value.ToString());

                // Vertical scale multiplier
                if (keys.Contains(App.ScaleYAppKey))
                    config.AppSettings.Settings[App.ScaleYAppKey].Value = nudVerticalMagnificationMultiplier.Value.ToString();
                else
                    config.AppSettings.Settings.Add(App.ScaleYAppKey, nudVerticalMagnificationMultiplier.Value.ToString());

                // Save the config
                config.Save();
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving the app setting, app.config may be corrupted.", "GMare", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }

            // If the undo/redo maximum has changed, update required
            _updateUndoRedo = _undoRedoMaximum != (int)nudMaximumUndoRedo.Value;

            // If the brightness or transparency has changed, texture updates are required
            if (_brightness != (float)nudLowerLayerBrightness.Value || _transparency != (float)nudUpperLayerTransparency.Value)
                _updateTextures = true;

            // Close dialog
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Cancel button click
        /// </summary>
        private void butCancel_Click(object sender, EventArgs e)
        {
            // Close dialog
            DialogResult = DialogResult.Cancel;
        }

        #endregion
    }
}