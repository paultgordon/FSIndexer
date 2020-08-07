namespace FSIndexer
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemMove = new System.Windows.Forms.ToolStripMenuItem();
            this.moveStandardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doNotRememberLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveGoodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveGreatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRename = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTermToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreTermToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRemoveNumbers = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRemoveNumbersAtStart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRemoveNumbersAtEnd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddNote = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSetRating = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemConvert = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddTag = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemGoogle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemResetAutoMove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.termMustStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rtbExecuteWindow = new System.Windows.Forms.RichTextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnAutoFile = new System.Windows.Forms.Button();
            this.cbFilterOnTypes = new System.Windows.Forms.ComboBox();
            this.numFilterOnChildrenSize = new System.Windows.Forms.NumericUpDown();
            this.numFilterOnTermLength = new System.Windows.Forms.NumericUpDown();
            this.btnGlobalReplace = new System.Windows.Forms.Button();
            this.numFilterOnMinimumSize = new System.Windows.Forms.NumericUpDown();
            this.numFilterOnAge = new System.Windows.Forms.NumericUpDown();
            this.cbSort = new System.Windows.Forms.ComboBox();
            this.btnRemoveDups = new System.Windows.Forms.Button();
            this.cbInvert = new System.Windows.Forms.CheckBox();
            this.btnResetHashes = new System.Windows.Forms.Button();
            this.mtvTerms = new FSIndexer.MultiSelectTreeview();
            this.btnClearTrash = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFilterOnChildrenSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFilterOnTermLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFilterOnMinimumSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFilterOnAge)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemMove,
            this.toolStripMenuItemRename,
            this.removeTermToolStripMenuItem,
            this.ignoreTermToolStripMenuItem,
            this.toolStripMenuItemRemoveNumbers,
            this.toolStripMenuItemAddNote,
            this.toolStripMenuItemSetRating,
            this.toolStripSeparator1,
            this.actionsToolStripMenuItem,
            this.toolStripMenuItemResetAutoMove,
            this.toolStripSeparator2,
            this.termMustStartToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(170, 236);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // toolStripMenuItemMove
            // 
            this.toolStripMenuItemMove.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveStandardToolStripMenuItem,
            this.moveGoodToolStripMenuItem,
            this.moveGreatToolStripMenuItem});
            this.toolStripMenuItemMove.Name = "toolStripMenuItemMove";
            this.toolStripMenuItemMove.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItemMove.Text = "Move";
            this.toolStripMenuItemMove.Click += new System.EventHandler(this.btnMoveSelected_Click);
            this.toolStripMenuItemMove.DoubleClick += new System.EventHandler(this.btnMoveSelected_Click);
            // 
            // moveStandardToolStripMenuItem
            // 
            this.moveStandardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.doNotRememberLocationToolStripMenuItem});
            this.moveStandardToolStripMenuItem.Name = "moveStandardToolStripMenuItem";
            this.moveStandardToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.moveStandardToolStripMenuItem.Text = "Standard";
            this.moveStandardToolStripMenuItem.Click += new System.EventHandler(this.btnMoveSelected_Click);
            this.moveStandardToolStripMenuItem.DoubleClick += new System.EventHandler(this.btnMoveSelected_Click);
            // 
            // doNotRememberLocationToolStripMenuItem
            // 
            this.doNotRememberLocationToolStripMenuItem.Name = "doNotRememberLocationToolStripMenuItem";
            this.doNotRememberLocationToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.doNotRememberLocationToolStripMenuItem.Text = "Do Not Remember Location";
            this.doNotRememberLocationToolStripMenuItem.Click += new System.EventHandler(this.btnMoveSelectedDoNotRemember_Click);
            // 
            // moveGoodToolStripMenuItem
            // 
            this.moveGoodToolStripMenuItem.Name = "moveGoodToolStripMenuItem";
            this.moveGoodToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.moveGoodToolStripMenuItem.Text = "With Good Shortcut";
            this.moveGoodToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItemMoveAndShortcut_Click);
            // 
            // moveGreatToolStripMenuItem
            // 
            this.moveGreatToolStripMenuItem.Name = "moveGreatToolStripMenuItem";
            this.moveGreatToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.moveGreatToolStripMenuItem.Text = "With Great Shortcut";
            this.moveGreatToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItemMoveAndShortcutStar_Click);
            // 
            // toolStripMenuItemRename
            // 
            this.toolStripMenuItemRename.Name = "toolStripMenuItemRename";
            this.toolStripMenuItemRename.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItemRename.Text = "Rename";
            this.toolStripMenuItemRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // removeTermToolStripMenuItem
            // 
            this.removeTermToolStripMenuItem.Name = "removeTermToolStripMenuItem";
            this.removeTermToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.removeTermToolStripMenuItem.Text = "Remove Term";
            this.removeTermToolStripMenuItem.Click += new System.EventHandler(this.btnRemoveTermFromFiles_Click);
            // 
            // ignoreTermToolStripMenuItem
            // 
            this.ignoreTermToolStripMenuItem.Name = "ignoreTermToolStripMenuItem";
            this.ignoreTermToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.ignoreTermToolStripMenuItem.Text = "Ignore Term";
            this.ignoreTermToolStripMenuItem.Click += new System.EventHandler(this.ignoreTermToolStripMenuItem_Click);
            // 
            // toolStripMenuItemRemoveNumbers
            // 
            this.toolStripMenuItemRemoveNumbers.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRemoveNumbersAtStart,
            this.toolStripMenuItemRemoveNumbersAtEnd});
            this.toolStripMenuItemRemoveNumbers.Name = "toolStripMenuItemRemoveNumbers";
            this.toolStripMenuItemRemoveNumbers.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItemRemoveNumbers.Text = "Remove Numbers";
            this.toolStripMenuItemRemoveNumbers.Click += new System.EventHandler(this.toolStripMenuItemRemoveNumbers_Click);
            // 
            // toolStripMenuItemRemoveNumbersAtStart
            // 
            this.toolStripMenuItemRemoveNumbersAtStart.Name = "toolStripMenuItemRemoveNumbersAtStart";
            this.toolStripMenuItemRemoveNumbersAtStart.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemRemoveNumbersAtStart.Text = "At Start Only";
            this.toolStripMenuItemRemoveNumbersAtStart.Click += new System.EventHandler(this.toolStripMenuItemRemoveNumbersAtStart_Click);
            // 
            // toolStripMenuItemRemoveNumbersAtEnd
            // 
            this.toolStripMenuItemRemoveNumbersAtEnd.Name = "toolStripMenuItemRemoveNumbersAtEnd";
            this.toolStripMenuItemRemoveNumbersAtEnd.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemRemoveNumbersAtEnd.Text = "At End Only";
            this.toolStripMenuItemRemoveNumbersAtEnd.Click += new System.EventHandler(this.toolStripMenuItemRemoveNumbersAtEnd_Click);
            // 
            // toolStripMenuItemAddNote
            // 
            this.toolStripMenuItemAddNote.Name = "toolStripMenuItemAddNote";
            this.toolStripMenuItemAddNote.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItemAddNote.Text = "Add Note";
            this.toolStripMenuItemAddNote.Click += new System.EventHandler(this.toolStripMenuItemAddNote_Click);
            // 
            // toolStripMenuItemSetRating
            // 
            this.toolStripMenuItemSetRating.Name = "toolStripMenuItemSetRating";
            this.toolStripMenuItemSetRating.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItemSetRating.Text = "Set Rating";
            this.toolStripMenuItemSetRating.Click += new System.EventHandler(this.toolStripMenuItemSetRating_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemConvert,
            this.toolStripMenuItemOpenFolder,
            this.toolStripMenuItemAddTag,
            this.toolStripMenuItemGoogle});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.actionsToolStripMenuItem.Text = "Actions";
            // 
            // toolStripMenuItemConvert
            // 
            this.toolStripMenuItemConvert.Name = "toolStripMenuItemConvert";
            this.toolStripMenuItemConvert.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemConvert.Text = "Convert";
            this.toolStripMenuItemConvert.Click += new System.EventHandler(this.toolStripMenuItemConvert_Click);
            // 
            // toolStripMenuItemOpenFolder
            // 
            this.toolStripMenuItemOpenFolder.Name = "toolStripMenuItemOpenFolder";
            this.toolStripMenuItemOpenFolder.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemOpenFolder.Text = "Open Folder";
            this.toolStripMenuItemOpenFolder.Click += new System.EventHandler(this.toolStripMenuItemOpenFolder_Click);
            // 
            // toolStripMenuItemAddTag
            // 
            this.toolStripMenuItemAddTag.Name = "toolStripMenuItemAddTag";
            this.toolStripMenuItemAddTag.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemAddTag.Text = "Add Tag";
            this.toolStripMenuItemAddTag.Click += new System.EventHandler(this.toolStripMenuItemAddTag_Click);
            // 
            // toolStripMenuItemGoogle
            // 
            this.toolStripMenuItemGoogle.Name = "toolStripMenuItemGoogle";
            this.toolStripMenuItemGoogle.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemGoogle.Text = "Google Term";
            this.toolStripMenuItemGoogle.Click += new System.EventHandler(this.toolStripMenuItemGoogle_Click);
            // 
            // toolStripMenuItemResetAutoMove
            // 
            this.toolStripMenuItemResetAutoMove.Name = "toolStripMenuItemResetAutoMove";
            this.toolStripMenuItemResetAutoMove.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItemResetAutoMove.Text = "Reset Auto Move";
            this.toolStripMenuItemResetAutoMove.Click += new System.EventHandler(this.toolStripMenuItemResetAutoMove_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(166, 6);
            // 
            // termMustStartToolStripMenuItem
            // 
            this.termMustStartToolStripMenuItem.Checked = true;
            this.termMustStartToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.termMustStartToolStripMenuItem.Name = "termMustStartToolStripMenuItem";
            this.termMustStartToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.termMustStartToolStripMenuItem.Text = "Term Must Start";
            this.termMustStartToolStripMenuItem.Click += new System.EventHandler(this.termMustStartToolStripMenuItem_Click);
            // 
            // rtbExecuteWindow
            // 
            this.rtbExecuteWindow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbExecuteWindow.DetectUrls = false;
            this.rtbExecuteWindow.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbExecuteWindow.Location = new System.Drawing.Point(12, 475);
            this.rtbExecuteWindow.Name = "rtbExecuteWindow";
            this.rtbExecuteWindow.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Horizontal;
            this.rtbExecuteWindow.Size = new System.Drawing.Size(1053, 125);
            this.rtbExecuteWindow.TabIndex = 5;
            this.rtbExecuteWindow.Text = "";
            this.rtbExecuteWindow.TextChanged += new System.EventHandler(this.rtbLogging_TextChanged);
            this.rtbExecuteWindow.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbExecuteWindow_KeyDown);
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Enabled = false;
            this.btnExecute.Location = new System.Drawing.Point(1007, 12);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(58, 23);
            this.btnExecute.TabIndex = 9;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(102, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(52, 23);
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(160, 12);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(52, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAutoFile
            // 
            this.btnAutoFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoFile.Location = new System.Drawing.Point(933, 12);
            this.btnAutoFile.Name = "btnAutoFile";
            this.btnAutoFile.Size = new System.Drawing.Size(68, 23);
            this.btnAutoFile.TabIndex = 15;
            this.btnAutoFile.Text = "Auto File";
            this.btnAutoFile.UseVisualStyleBackColor = true;
            this.btnAutoFile.Click += new System.EventHandler(this.btnAutoFile_Click);
            // 
            // cbFilterOnTypes
            // 
            this.cbFilterOnTypes.FormattingEnabled = true;
            this.cbFilterOnTypes.Location = new System.Drawing.Point(245, 14);
            this.cbFilterOnTypes.Name = "cbFilterOnTypes";
            this.cbFilterOnTypes.Size = new System.Drawing.Size(71, 21);
            this.cbFilterOnTypes.TabIndex = 17;
            this.cbFilterOnTypes.SelectedIndexChanged += new System.EventHandler(this.cbFilterOnTypes_SelectedIndexChanged);
            // 
            // numFilterOnChildrenSize
            // 
            this.numFilterOnChildrenSize.Location = new System.Drawing.Point(322, 15);
            this.numFilterOnChildrenSize.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numFilterOnChildrenSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFilterOnChildrenSize.Name = "numFilterOnChildrenSize";
            this.numFilterOnChildrenSize.Size = new System.Drawing.Size(40, 20);
            this.numFilterOnChildrenSize.TabIndex = 18;
            this.numFilterOnChildrenSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numFilterOnChildrenSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFilterOnChildrenSize.ValueChanged += new System.EventHandler(this.numFilterOnChildrenLessThan_ValueChanged);
            this.numFilterOnChildrenSize.DoubleClick += new System.EventHandler(this.numFilterOnChildrenSize_DoubleClick);
            // 
            // numFilterOnTermLength
            // 
            this.numFilterOnTermLength.Location = new System.Drawing.Point(368, 15);
            this.numFilterOnTermLength.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numFilterOnTermLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFilterOnTermLength.Name = "numFilterOnTermLength";
            this.numFilterOnTermLength.Size = new System.Drawing.Size(40, 20);
            this.numFilterOnTermLength.TabIndex = 19;
            this.numFilterOnTermLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numFilterOnTermLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFilterOnTermLength.ValueChanged += new System.EventHandler(this.numFilterOnTermLength_ValueChanged);
            this.numFilterOnTermLength.DoubleClick += new System.EventHandler(this.numFilterOnTermLength_DoubleClick);
            // 
            // btnGlobalReplace
            // 
            this.btnGlobalReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGlobalReplace.Location = new System.Drawing.Point(520, 12);
            this.btnGlobalReplace.Name = "btnGlobalReplace";
            this.btnGlobalReplace.Size = new System.Drawing.Size(96, 23);
            this.btnGlobalReplace.TabIndex = 21;
            this.btnGlobalReplace.Text = "Global Replace";
            this.btnGlobalReplace.UseVisualStyleBackColor = true;
            this.btnGlobalReplace.Click += new System.EventHandler(this.btnGlobalReplace_Click);
            // 
            // numFilterOnMinimumSize
            // 
            this.numFilterOnMinimumSize.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numFilterOnMinimumSize.Location = new System.Drawing.Point(414, 15);
            this.numFilterOnMinimumSize.Maximum = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            this.numFilterOnMinimumSize.Name = "numFilterOnMinimumSize";
            this.numFilterOnMinimumSize.Size = new System.Drawing.Size(54, 20);
            this.numFilterOnMinimumSize.TabIndex = 23;
            this.numFilterOnMinimumSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numFilterOnMinimumSize.ThousandsSeparator = true;
            this.numFilterOnMinimumSize.ValueChanged += new System.EventHandler(this.numFilterOnMinimumSize_ValueChanged);
            this.numFilterOnMinimumSize.DoubleClick += new System.EventHandler(this.numFilterOnMinimumSize_DoubleClick);
            // 
            // numFilterOnAge
            // 
            this.numFilterOnAge.Location = new System.Drawing.Point(474, 15);
            this.numFilterOnAge.Maximum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.numFilterOnAge.Name = "numFilterOnAge";
            this.numFilterOnAge.Size = new System.Drawing.Size(40, 20);
            this.numFilterOnAge.TabIndex = 24;
            this.numFilterOnAge.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numFilterOnAge.ValueChanged += new System.EventHandler(this.numFilterOnAge_ValueChanged);
            this.numFilterOnAge.DoubleClick += new System.EventHandler(this.numFilterOnAge_DoubleClick);
            // 
            // cbSort
            // 
            this.cbSort.FormattingEnabled = true;
            this.cbSort.Location = new System.Drawing.Point(12, 14);
            this.cbSort.Name = "cbSort";
            this.cbSort.Size = new System.Drawing.Size(80, 21);
            this.cbSort.TabIndex = 25;
            this.cbSort.SelectedIndexChanged += new System.EventHandler(this.cbSort_SelectedIndexChanged);
            // 
            // btnRemoveDups
            // 
            this.btnRemoveDups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveDups.Location = new System.Drawing.Point(717, 12);
            this.btnRemoveDups.Name = "btnRemoveDups";
            this.btnRemoveDups.Size = new System.Drawing.Size(94, 23);
            this.btnRemoveDups.TabIndex = 26;
            this.btnRemoveDups.Text = "Remove Dups";
            this.btnRemoveDups.UseVisualStyleBackColor = true;
            this.btnRemoveDups.Click += new System.EventHandler(this.btnRemoveDups_Click);
            // 
            // cbInvert
            // 
            this.cbInvert.Location = new System.Drawing.Point(215, 12);
            this.cbInvert.Margin = new System.Windows.Forms.Padding(0);
            this.cbInvert.Name = "cbInvert";
            this.cbInvert.Size = new System.Drawing.Size(30, 24);
            this.cbInvert.TabIndex = 27;
            this.cbInvert.Text = "!";
            this.cbInvert.UseVisualStyleBackColor = true;
            this.cbInvert.Visible = false;
            // 
            // btnResetHashes
            // 
            this.btnResetHashes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetHashes.Location = new System.Drawing.Point(622, 12);
            this.btnResetHashes.Name = "btnResetHashes";
            this.btnResetHashes.Size = new System.Drawing.Size(89, 23);
            this.btnResetHashes.TabIndex = 28;
            this.btnResetHashes.Text = "Reset Hashes";
            this.btnResetHashes.UseVisualStyleBackColor = true;
            this.btnResetHashes.Click += new System.EventHandler(this.btnResetHashes_Click);
            // 
            // mtvTerms
            // 
            this.mtvTerms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mtvTerms.ContextMenuStrip = this.contextMenuStrip;
            this.mtvTerms.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mtvTerms.FullRowSelect = true;
            this.mtvTerms.HideSelection = false;
            this.mtvTerms.Location = new System.Drawing.Point(12, 41);
            this.mtvTerms.Name = "mtvTerms";
            this.mtvTerms.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("mtvTerms.SelectedNodes")));
            this.mtvTerms.ShowNodeToolTips = true;
            this.mtvTerms.Size = new System.Drawing.Size(1053, 428);
            this.mtvTerms.TabIndex = 20;
            this.mtvTerms.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TV_NodeMouseClick);
            this.mtvTerms.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TV_NodeMouseDoubleClick);
            this.mtvTerms.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TV_KeyDown);
            // 
            // btnClearTrash
            // 
            this.btnClearTrash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearTrash.Location = new System.Drawing.Point(817, 12);
            this.btnClearTrash.Name = "btnClearTrash";
            this.btnClearTrash.Size = new System.Drawing.Size(80, 23);
            this.btnClearTrash.TabIndex = 29;
            this.btnClearTrash.Text = "Clear Trash";
            this.btnClearTrash.UseVisualStyleBackColor = true;
            this.btnClearTrash.Click += new System.EventHandler(this.btnClearTrash_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1077, 612);
            this.Controls.Add(this.btnClearTrash);
            this.Controls.Add(this.btnResetHashes);
            this.Controls.Add(this.cbInvert);
            this.Controls.Add(this.btnRemoveDups);
            this.Controls.Add(this.cbSort);
            this.Controls.Add(this.numFilterOnAge);
            this.Controls.Add(this.numFilterOnMinimumSize);
            this.Controls.Add(this.numFilterOnTermLength);
            this.Controls.Add(this.btnGlobalReplace);
            this.Controls.Add(this.numFilterOnChildrenSize);
            this.Controls.Add(this.cbFilterOnTypes);
            this.Controls.Add(this.btnAutoFile);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.rtbExecuteWindow);
            this.Controls.Add(this.mtvTerms);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TV_KeyDown);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numFilterOnChildrenSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFilterOnTermLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFilterOnMinimumSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFilterOnAge)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbExecuteWindow;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMove;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRename;
        private System.Windows.Forms.Button btnAutoFile;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemResetAutoMove;
        private System.Windows.Forms.ComboBox cbFilterOnTypes;
        private System.Windows.Forms.NumericUpDown numFilterOnChildrenSize;
        private System.Windows.Forms.NumericUpDown numFilterOnTermLength;
        private MultiSelectTreeview mtvTerms;
        private System.Windows.Forms.ToolStripMenuItem moveStandardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveGoodToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveGreatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeTermToolStripMenuItem;
        private System.Windows.Forms.Button btnGlobalReplace;
        private System.Windows.Forms.ToolStripMenuItem doNotRememberLocationToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown numFilterOnMinimumSize;
        private System.Windows.Forms.NumericUpDown numFilterOnAge;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemoveNumbers;
        private System.Windows.Forms.ToolStripMenuItem ignoreTermToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemConvert;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGoogle;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem termMustStartToolStripMenuItem;
        private System.Windows.Forms.ComboBox cbSort;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemoveNumbersAtStart;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemoveNumbersAtEnd;
        private System.Windows.Forms.Button btnRemoveDups;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddTag;
        private System.Windows.Forms.CheckBox cbInvert;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddNote;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetRating;
        private System.Windows.Forms.Button btnResetHashes;
        private System.Windows.Forms.Button btnClearTrash;
    }
}

