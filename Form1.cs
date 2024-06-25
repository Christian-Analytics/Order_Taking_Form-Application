// Programmer: Christian Castillo
// Description: Form for taking balloon orders
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using System.IO; // Allows us to use StreamReader


namespace Castillo_3
{
    public partial class Form1 : Form
    {
        // Declaring class level constants for pricing
        private const decimal HOME_DELIVERY_UPCHARGE = 7.5m;
        private const decimal SINGLE_BUNDLE_COST = 9.95m;
        private const decimal HALF_DOZEN_BUNDLE_COST = 35.95m;
        private const decimal DOZEN_BUNDLE_COST = 65.95m;
        private const decimal EXTRAS_UPCHARGE = 9.5m;
        private const decimal PERSONALIZED_MESSAGE_UPCHARGE = 2.5m;
        private const decimal TAX_RATE = 0.07m;

        // Declaring class level variables for use in calculating totals
        private decimal subtotal = 0m;
        private decimal salesTax = 0m;
        private decimal orderTotal = 0m;
        private decimal baseSubtotal = 0.0m;

        public Form1()
        {
            InitializeComponent();
        }

        // Handle form start up event handler to populate boxes displaying costs and update totals based on default selections
        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateBoxes();
            UpdateTotals();
        }

        // Handle the personalized message check box's event handler
        private void personalizedMessageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Enables the check box and its instruction label when it's selected
            if (personalizedMessageCheckBox.Checked)
            {
                personalizedMessageInstructionLabel.Enabled = true;
                personalizedMessageTextBox.Enabled = true;
            }
            else
            {
                personalizedMessageInstructionLabel.Enabled = false;
                personalizedMessageTextBox.Enabled = false;
                personalizedMessageTextBox.Text = "";
            }

            // Updates total to account for upcharge when selecting a personalized message
            UpdateTotals();

        }

        // Set store pick up's event handler to update totals to account for home delivery upcharge
        private void storePickUpRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        // Handle bundle selection's event handler to update totals to account for different balloon quantities
        private void singleBundleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        // Handle half dozen bundle selection to update totals (only need to set 2 checked changed events since there's only 3 radio buttons
        private void halfDozenBundleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        // Handle extras list box event handler to account for the extras upcharge
        private void extrasListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        // Handles clear button's event handler to reset form to its original state
        private void clearFormButton_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        // Handle exit button's event handler to open a MessageBox to confirm program closure
        private void exitProgramButton_Click(object sender, EventArgs e)
        {
            DialogResult result= MessageBox.Show("Are you sure you want to exit?",
               "Exit Confirmation",  
                MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes) // If user confirms they want to close the program, program closes
            {
                this.Close();
            }
        }

        // Handles display summary button's event handler to display a summary of the order
        private void displaySummaryButton_Click(object sender, EventArgs e)
        {
            // Declares strings for use in MessageBox.Show method from radio buttons selected or listbox indices selected
            string deliveryType = "";
            string bundleSize = "";
            string extra = "";

            try // Attempts to assign string variables for use in MessageBox.Show method and run the MessageBox
            {

                // Assigns delivery type string for use in MessageBox.Show method based on radio button selected
                if (storePickUpRadioButton.Checked)
                {
                    deliveryType = storePickUpRadioButton.Text;
                }
                else
                {
                    deliveryType = homeDeliveryRadioButton.Text;
                }

                // Assigns bundle size string for use in MessageBox.Show method based on what radio button is selected by user
                if (singleBundleRadioButton.Checked)
                {
                    bundleSize = singleBundleRadioButton.Text;
                }
                else if (halfDozenBundleRadioButton.Checked)
                {
                    bundleSize = halfDozenBundleRadioButton.Text;
                }
                else if (dozenBundleRadioButton.Checked)
                {
                    bundleSize = dozenBundleRadioButton.Text;
                }

                // Assigns a value to extra string based on whatever is selected in the list box by the user
                // Uses foreach loop covered in Chapter 7 to add each selected item
                // Loop variable "selectedItem" iterates for each item that is selected from the list box and then added as a string variable for use in MessageBox.Show method
                foreach (string selectedItem in extrasListBox.SelectedItems)
                {
                    extra += selectedItem + "\n";.
                }

                // Display an error message if customer name fields are left blank or phone number field is incomplete
                if (firstNameTextBox.Text == "" || lastNameTextBox.Text == "" || phoneNumberMaskedTextBox.MaskCompleted == false)
                {
                    MessageBox.Show("Please ensure first and last name fields are not left blank and phone number is properly inputted.",
                        "Fields left blank", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else // Display a Message Box showing a summary of the order as typed/selected by the user
                {
                    //  Message Box with multiple lines with Order Summary as the title header and \n used to create new lines
                    MessageBox.Show("Bonnie's Balloons Order Summary\n" +
                        "Name: " + titleComboBox.Text + " " + firstNameTextBox.Text + " " + lastNameTextBox.Text + "\n" +
                        "Address: " + streetTextBox.Text + cityTextBox.Text + stateMaskedTextBox.Text + zipCodeMaskedTextBox.Text + "\n" +
                        "Phone Number: " + phoneNumberMaskedTextBox.Text + "\n" +
                        "Delivery Date: " + deliveryDateMaskedTextBox.Text + "\n" +
                        "Delivery type: " + deliveryType + "\n" +
                        "Bundle Size: " + bundleSize + "\n" +
                        "Special Occasion: " + specialOccasionComboBox.Text + "\n" +
                        "Extras (if any): " + extra + "\n" +
                        personalizedMessageTextBox.Text + "\n" +
                        "Order Subtotal: " + orderSubtotalLabel.Text + "\n" +
                        "Sales Tax: " + salesTaxAmountLabel.Text + "\n" +
                        "Order Total: " + orderTotalLabel.Text + "\n",
                        "Order Summary", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    // Form is cleared and returned to its original state after displaying order summary
                    ResetForm();
                }
            }
            catch(Exception ex)
            { 
                MessageBox.Show(ex.Message); // Using catch to display a message if there's any errors with the assignment of variables (declared for use in MessageBox.Show method
            }
        }

        // Reads data from external files into the special occasion ComboBox and the extras ListBox
        // Inputs the date in the delivery date masked text box as the default
        // Use To String method to show costs on form as currency
        private void PopulateBoxes()
        {
            StreamReader inputFile; // Declaring Object
            try
            {
                specialOccasionComboBox.Items.Clear(); // Clears any existing items from the special occassion combo box
                inputFile = File.OpenText("Occasions.txt"); // Opens occasions file
                while (!inputFile.EndOfStream) // Keeps reading the data until there's no more data
                {
                    specialOccasionComboBox.Items.Add(inputFile.ReadLine());
                }
                inputFile.Close();
                specialOccasionComboBox.SelectedItem = "Birthdays"; //Ensures Birthdays is the default selection

                extrasListBox.Items.Clear(); // Clears any existing items from the extras list box
                inputFile = File.OpenText("Extras.txt"); // Opens extras file
                while (!inputFile.EndOfStream) // Keeps reading until there's no more data
                {
                    extrasListBox.Items.Add(inputFile.ReadLine()); //Reads data to list box
                }
                inputFile.Close();
            }
            catch (Exception ex)
            {
                // Display message if error occurs when attempting to read the file
                MessageBox.Show(ex.Message);
            }

            // Display current date under delivery date as reported by the system clock
            deliveryDateMaskedTextBox.Text = DateTime.Now.ToString("MM/dd/yyyy");

            // Display costs on form
            personalizedMessagePriceLabel.Text = PERSONALIZED_MESSAGE_UPCHARGE.ToString("c");
            homeDeliveryUpchargeLabel.Text = HOME_DELIVERY_UPCHARGE.ToString("c");
            singleBundleCostLabel.Text = SINGLE_BUNDLE_COST.ToString("c");
            halfDozenBundleCostLabel.Text = HALF_DOZEN_BUNDLE_COST.ToString("c");
            dozenBundleCostLabel.Text = DOZEN_BUNDLE_COST.ToString("c");
            boxOfChocolatesPriceLabel.Text = EXTRAS_UPCHARGE.ToString("c");
            coffeeMugPriceLabel.Text = EXTRAS_UPCHARGE.ToString("c");
            flowerArrangementPriceLabel.Text = EXTRAS_UPCHARGE.ToString("c");
            jarOfJellyBeansPriceLabel.Text = EXTRAS_UPCHARGE.ToString("c");
            pottedPlantPriceLabel.Text = EXTRAS_UPCHARGE.ToString("c");
            taxRateLabel.Text = "(" + TAX_RATE.ToString("p") + ")"; // Shows tax rate as a percentage rather than currency format
        }

        // Resets the form to its original format
        private void ResetForm()
        {
            titleComboBox.SelectedIndex = -1; // No title selected for customer
            titleComboBox.Text = null;
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            phoneNumberMaskedTextBox.Text = "";
            streetTextBox.Text = "";
            stateMaskedTextBox.Text = "";
            cityTextBox.Text = "";
            zipCodeMaskedTextBox.Text = "";
            deliveryDateMaskedTextBox.Text = DateTime.Now.ToString("MM/dd/yyyy"); // Restores date in system in case date was changed
            storePickUpRadioButton.Checked = true;
            singleBundleRadioButton.Checked = true;
            specialOccasionComboBox.SelectedItem = "Birthdays";
            extrasListBox.SelectedItems.Clear();
            personalizedMessageCheckBox.Checked = false;

            titleComboBox.Focus(); // Sets focus to first data entry control
        }

        // Update totals whenever certain selections are made
        private void UpdateTotals()
        {
            subtotal = baseSubtotal;
            // Update total when home delivery is selected
            if (homeDeliveryRadioButton.Checked)
            {
                subtotal += HOME_DELIVERY_UPCHARGE;
            }

            // Update total when bundle size is adjusted
            if (singleBundleRadioButton.Checked)
            {
                subtotal += SINGLE_BUNDLE_COST;
            }
            else if (halfDozenBundleRadioButton.Checked)
            {
                subtotal += HALF_DOZEN_BUNDLE_COST;
            }
            else if (dozenBundleRadioButton.Checked)
            {
                subtotal += DOZEN_BUNDLE_COST;
            }

            // Update total if customer chooses a personalized message box
            if (personalizedMessageCheckBox.Checked)
            {
                subtotal += PERSONALIZED_MESSAGE_UPCHARGE;
            }

            // Use switch method to add extras upcharge if any indices are selected 
            switch (extrasListBox.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    // Adds the extras upcharge times however many indices are selected
                    // Switch method only adds it once so it has to be multiplied by selected index count
                    subtotal += EXTRAS_UPCHARGE * extrasListBox.SelectedIndices.Count;
                    break;
            }

            // Assigns sales tax amount based on subtotal to local variable
            salesTax = subtotal * TAX_RATE;

            // Assign order total amount as pre-tax subtotal plus sales Tax amount
            orderTotal = subtotal + salesTax;

            // Display subtotal and sales tax amount and total amount to appropriate controls as strings under currency format
            orderSubtotalLabel.Text = subtotal.ToString("c");
            salesTaxAmountLabel.Text = salesTax.ToString("c");
            orderTotalLabel.Text = orderTotal.ToString("c");

        }
    }
}

