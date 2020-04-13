var customerLevelViewModel = function () {
    var self = this;
    self.headerTitle = ko.observable('Customer level configuration');
    self.listCustomerLevel = ko.observable('List of customer levels'); 
    
    //====html===
    self.add=ko.observable('<a href="/CustomerConfig/CreateCustomerLevel" role="button" class="btn btn-success pull-right"><i class="fa fa-plus"></i> Add new</a>'); // HTML content appears
 

}
ko.applyBindings(new customerLevelViewModel());